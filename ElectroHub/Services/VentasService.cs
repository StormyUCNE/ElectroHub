using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        // Agregamos la venta al contexto
        contexto.Ventas.Add(venta);
        // Guardamos para que se genere el ID de la venta
        var resultado = await contexto.SaveChangesAsync() > 0;
        return resultado;
    }

    private async Task<bool> Modificar(Ventas venta)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        // Para modificaciones de ventas, generalmente no se permite modificar
        // detalles ya que son transacciones cerradas. Podrías manejar devoluciones
        // como notas de crédito o ventas negativas.

        // Aquí simplemente actualizamos los campos de la venta que pueden cambiar
        // como método de pago, pero no los detalles ni el total
        var ventaExistente = await contexto.Ventas
            .Include(v => v.DetallesVentas)
            .FirstOrDefaultAsync(v => v.VentaId == venta.VentaId);

        if (ventaExistente == null)
            return false;

        // Actualizar solo campos permitidos
        ventaExistente.MetodoPago = venta.MetodoPago;
        ventaExistente.MontoRecibido = venta.MontoRecibido;
        ventaExistente.Vuelto = venta.MontoRecibido - ventaExistente.Total;

        contexto.Ventas.Update(ventaExistente);
        return await contexto.SaveChangesAsync() > 0;
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
            .FirstOrDefaultAsync(v => v.VentaId == ventaId && !v.Eliminado);
    }

    public async Task<bool> Eliminar(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        // Soft delete - marcar como eliminado
        var filasAfectadas = await contexto.Ventas!
            .Where(v => v.VentaId == ventaId)
            .ExecuteUpdateAsync(v =>
                v.SetProperty(x => x.Eliminado, true));
        return filasAfectadas > 0;
    }

    public async Task<List<Ventas>> Listar(Expression<Func<Ventas, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(v => v.DetallesVentas)
                .ThenInclude(d => d.Productos)
            .Where(criterio)
            .Where(v => !v.Eliminado)
            .ToListAsync();
    }

    // Método adicional para calcular los totales de una venta
    public Ventas CalcularTotales(Ventas venta, decimal itbisPorcentaje = 18)
    {
        // Calcular subtotal (suma de cantidad * precio de cada detalle)
        venta.Subtotal = venta.DetallesVentas.Sum(d => d.Cantidad * d.Precio);

        // Calcular descuento aplicado
        venta.DescuentoAplicado = venta.Subtotal * (venta.Descuento / 100);

        // Calcular ITBIS sobre el subtotal con descuento
        venta.Itbis = (venta.Subtotal - venta.DescuentoAplicado) * (itbisPorcentaje / 100);

        // Calcular total
        venta.Total = venta.Subtotal - venta.DescuentoAplicado + venta.Itbis;

        // Calcular vuelto
        if (venta.MontoRecibido != 0)
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
            .FirstOrDefaultAsync(p => p.ProductoId == productoId);

        if (producto == null)
            throw new ArgumentException("Producto no encontrado");

        // Calcular ITBIS (asumiendo 18% como ejemplo)
        decimal itbisPorcentaje = 18;
        decimal precioConItbis = producto.PrecioVenta.Value * (1 + itbisPorcentaje / 100);
        decimal itbis = producto.PrecioVenta.Value * (itbisPorcentaje / 100);

        return new DetallesVentas
        {
            ProductoId = productoId,
            Productos = producto,
            Descripcion = producto.Descripcion,
            Categoria = producto.Categorias,
            Cantidad = cantidad,
            Precio = producto.PrecioVenta.Value,
            Itbis = itbis,
            Subtotal = producto.PrecioVenta.Value * cantidad
        };
    }
}