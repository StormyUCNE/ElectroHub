using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ElectroHub.Services;

public class VentasService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Guardar(Ventas venta)
    {
        if (!await Existe(venta.VentaId))
            return await Insertar(venta);
        else
            return await Modificar(venta);
    }

    private async Task<bool> Existe(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas.AnyAsync(v => v.VentaId == ventaId);
    }

    private async Task<bool> Insertar(Ventas venta)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        // Crear una NUEVA venta sin las entidades de navegación
        var ventaNueva = new Ventas
        {
            Fecha = venta.Fecha,
            MetodoPago = venta.MetodoPago,
            Vendedor = venta.Vendedor,
            MontoRecibido = venta.MontoRecibido,
            Descuento = venta.Descuento,
            Subtotal = venta.Subtotal,
            Itbis = venta.Itbis,
            Total = venta.Total,
            DescuentoAplicado = venta.DescuentoAplicado,
            Vuelto = venta.Vuelto,
            Eliminado = venta.Eliminado
        };

        // Agregar detalles SIN las entidades de navegación
        foreach (var detalle in venta.DetallesVentas)
        {
            ventaNueva.DetallesVentas.Add(new DetallesVentas
            {
                ProductoId = detalle.ProductoId,
                CategoriaId = detalle.CategoriaId,
                Descripcion = detalle.Descripcion,
                Cantidad = detalle.Cantidad,
                Precio = detalle.Precio,
                Itbis = detalle.Itbis,
                Subtotal = detalle.Subtotal,
                Eliminado = detalle.Eliminado
            });
        }

        await AfectarProductos(contexto, venta.DetallesVentas.ToArray(), TipoOperacion.Resta, venta.Vendedor);

        contexto.Ventas.Add(ventaNueva);

        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Ventas venta)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var ventaExistente = await contexto.Ventas
            .AsNoTracking()
            .Include(v => v.DetallesVentas)
            .FirstOrDefaultAsync(v => v.VentaId == venta.VentaId);

        if (ventaExistente == null)
            return false;

        await AfectarProductos(contexto, ventaExistente.DetallesVentas.ToArray(), TipoOperacion.Suma, venta.Vendedor);

        // Eliminar detalles existentes
        contexto.DetallesVentas.RemoveRange(ventaExistente.DetallesVentas);
        ventaExistente.DetallesVentas.Clear();
        foreach (var detalle in venta.DetallesVentas)
        {
            ventaExistente.DetallesVentas.Add(new DetallesVentas
            {
                ProductoId = detalle.ProductoId,
                CategoriaId = detalle.CategoriaId,
                Descripcion = detalle.Descripcion,
                Cantidad = detalle.Cantidad,
                Precio = detalle.Precio,
                Itbis = detalle.Itbis,
                Subtotal = detalle.Subtotal,
                Eliminado = detalle.Eliminado
            });
        }

        await AfectarProductos(contexto, ventaExistente.DetallesVentas.ToArray(), TipoOperacion.Resta, venta.Vendedor);

        // Actualizar solo campos permitidos
        ventaExistente.MetodoPago = venta.MetodoPago;
        ventaExistente.MontoRecibido = venta.MontoRecibido;
        ventaExistente.Descuento = venta.Descuento;
        ventaExistente.Eliminado = venta.Eliminado;
        ventaExistente.Fecha = venta.Fecha;

        CalcularTotales(ventaExistente);

        contexto.Ventas.Update(ventaExistente);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task AfectarProductos(ApplicationDbContext contexto, DetallesVentas[] detalles, TipoOperacion tipoOperacion, string nombreUsuario)
    {
        foreach (var detalle in detalles)
        {
            Productos producto = await contexto.Productos.FirstOrDefaultAsync(p => p.ProductoId == detalle.ProductoId);
            if (producto == null) return;

            int stockActual = producto.CantidadInventario ?? detalle.Cantidad;
            if (tipoOperacion == TipoOperacion.Suma)
            {
                int stockNuevo = stockActual + detalle.Cantidad;
                producto.CantidadInventario = stockNuevo;

                // Agregar movimiento en Inventario
                var movimiento = new InventarioMovimientos
                {
                    ProductoId = detalle.ProductoId,
                    FechaMovimiento = DateTime.Now,
                    TipoMovimiento = "Ajuste",
                    Cantidad = detalle.Cantidad,
                    StockResultante = stockNuevo,
                    Usuario = nombreUsuario
                };
                contexto.InventarioMovimientos.Add(movimiento);
            }
            if (tipoOperacion == TipoOperacion.Resta)
            {
                int stockNuevo = stockActual - detalle.Cantidad;
                producto.CantidadInventario = stockNuevo;

                // Agregar movimiento en Inventario
                var movimiento = new InventarioMovimientos
                {
                    ProductoId = detalle.ProductoId,
                    FechaMovimiento = DateTime.Now,
                    TipoMovimiento = "Salida",
                    Cantidad = detalle.Cantidad * (-1),
                    StockResultante = stockNuevo,
                    Usuario = nombreUsuario
                };
                contexto.InventarioMovimientos.Add(movimiento);
            }
        }
    }

    public async Task<Ventas?> Buscar(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(v => v.DetallesVentas)
                .ThenInclude(d => d.Productos)
                    .ThenInclude(p => p.Categorias)
            .Include(v => v.DetallesVentas)
                .ThenInclude(d => d.Productos)
                    .ThenInclude(p => p.Proveedores)
            .Include(v => v.DetallesVentas)
                .ThenInclude(d => d.Categoria)
            .FirstOrDefaultAsync(v => v.VentaId == ventaId);
    }

    public async Task<bool> Eliminar(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        Ventas venta = await Buscar(ventaId);
        await AfectarProductos(contexto, venta.DetallesVentas.ToArray(), TipoOperacion.Suma, venta.Vendedor);
        // Soft delete
        await contexto.Ventas!.Where(v => v.VentaId == ventaId).ExecuteUpdateAsync(v => v.SetProperty(x => x.Eliminado, true));
        return await contexto.SaveChangesAsync() > 0;
    }
    public async Task<bool> Recuperar(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        Ventas venta = await Buscar(ventaId);
        if (venta.Eliminado == false) return false;

        await AfectarProductos(contexto, venta.DetallesVentas.ToArray(), TipoOperacion.Resta, venta.Vendedor);
        // Anula Soft delete
        await contexto.Ventas!.Where(v => v.VentaId == ventaId).ExecuteUpdateAsync(v => v.SetProperty(x => x.Eliminado, false));
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<List<Ventas>> Listar(Expression<Func<Ventas, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(v => v.DetallesVentas)
                .ThenInclude(d => d.Productos)
            .Where(criterio)
            .ToListAsync();
    }

    public Ventas CalcularTotales(Ventas venta, decimal itbisPorcentaje = 18)
    {
        venta.Subtotal = venta.DetallesVentas.Sum(d => d.Cantidad * d.Precio);
        venta.DescuentoAplicado = venta.Subtotal * (venta.Descuento / 100);
        venta.Itbis = (venta.Subtotal - venta.DescuentoAplicado) * (itbisPorcentaje / 100);
        venta.Total = venta.Subtotal - venta.DescuentoAplicado + venta.Itbis;

        // Calcular vuelto
        if (venta.MontoRecibido > venta.Total)
        {
            venta.Vuelto = venta.MontoRecibido - venta.Total;
        }
        else
        {
            venta.Vuelto = 0;
        }

        return venta;
    }

    public async Task<DetallesVentas> CrearDetalleVenta(int productoId, int cantidad)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var producto = await contexto.Productos
            .Include(p => p.Categorias)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductoId == productoId);

        if (producto == null)
            throw new ArgumentException("Producto no encontrado");

        // Calcular ITBIS (asumiendo 18% como ejemplo)
        decimal itbisPorcentaje = 18;
        decimal precioConItbis = producto.PrecioVenta.Value * (1 + itbisPorcentaje / 100);
        decimal itbis = producto.PrecioVenta.Value * (itbisPorcentaje / 100);

        DetallesVentas detalle = new DetallesVentas
        {
            ProductoId = producto.ProductoId,
            CategoriaId = producto.CategoriaId,
            Descripcion = producto.Nombre,
            Cantidad = cantidad,
            Precio = producto.PrecioVenta.Value,
            Itbis = itbis,
            Subtotal = producto.PrecioVenta.Value * cantidad
        };

        // Solo para navegacion
        detalle.Productos = producto;
        detalle.Categoria = producto.Categorias;

        return detalle;
    }
}

public enum TipoOperacion
{
    Suma = 1,
    Resta = 2
}