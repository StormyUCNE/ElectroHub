using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroHub.Services;

public class ProductosService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Guardar(Productos producto)
    {
        if (!await Existe(producto.ProductoId))
            return await Insertar(producto);
        else
            return await Modificar(producto);
    }

    private async Task<bool> Existe(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos.AnyAsync(p => p.ProductoId == productoId);
    }

    private async Task<bool> Insertar(Productos producto)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        // 1. Agregamos el producto al contexto
        contexto.Productos.Add(producto);
        // Guardamos para que se genere el ID del producto si es Identity
        return await contexto.SaveChangesAsync() > 0;

        /*
        var movimiento = new InventarioMovimientos
        {
            ProductoId = producto.ProductoId,
            FechaMovimiento = DateTime.Now,
            TipoMovimiento = "Entrada",
            Cantidad = producto.CantidadInventario ?? 0,
            StockResultante = producto.CantidadInventario ?? 0,
            Usuario = "Sistema", // Puedes personalizar esto
        };

        contexto.InventarioMovimientos.Add(movimiento);
        return await contexto.SaveChangesAsync() > 0;
        */
    }

    private async Task<bool> Modificar(Productos producto)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        // 1. Buscamos el stock actual en la DB antes de actualizar para comparar
        var stockAnterior = await contexto.Productos
            .Where(p => p.ProductoId == producto.ProductoId)
            .Select(p => p.CantidadInventario)
            .FirstOrDefaultAsync() ?? 0;

        // 2. Actualizamos el producto
        contexto.Update(producto);

        // 3. Si la cantidad cambió, registramos el movimiento de auditoría
        if (stockAnterior != producto.CantidadInventario)
        {
            int nuevaCantidad = producto.CantidadInventario ?? 0;
            int diferencia = nuevaCantidad - stockAnterior;

            var movimiento = new InventarioMovimientos
            {
                ProductoId = producto.ProductoId,
                FechaMovimiento = DateTime.Now,
                // Si la diferencia es positiva es Entrada, si es negativa es Ajuste/Salida
                TipoMovimiento = diferencia > 0 ? "Entrada" : "Ajuste",
                Cantidad = diferencia,
                StockResultante = nuevaCantidad,
                Usuario = "Sistema"
            };
            contexto.InventarioMovimientos.Add(movimiento);
        }

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Productos?> Buscar(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos.FirstOrDefaultAsync(p => p.ProductoId == productoId);
    }

    public async Task<bool> Eliminar(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var filasAfectadas = await contexto.Productos!
            .Where(p => p.ProductoId == productoId)
            .ExecuteUpdateAsync(p =>
                p.SetProperty(x => x.Eliminado, true));

        return filasAfectadas > 0;
    }

    public async Task<List<Productos>> Listar(Expression<Func<Productos, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos
            .Include(c => c.Categorias)
            .Include(p => p.Proveedores)
            .Where(criterio)
            .ToListAsync();
    }
}