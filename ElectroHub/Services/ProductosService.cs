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
        contexto.Productos.Add(producto);
        return await contexto.SaveChangesAsync() > 0;
    }
    private async Task<bool> Modificar(Productos producto)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(producto);
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
        return await contexto.Productos.Include(c => c.Categorias).Include(p => p.Proveedores).Include(e => e.EstadosProductos).Where(criterio).ToListAsync();
    }
}