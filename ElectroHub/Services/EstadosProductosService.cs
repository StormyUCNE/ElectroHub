using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace ElectroHub.Services;
public class EstadosProductosService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Guardar(EstadosProductos estado)
    {
        if (await Existe(estado.EstadoProductoId))
            return false;
        if (!await Existe(estado.EstadoProductoId))
            return await Insertar(estado);
        else
            return await Modificar(estado);
    }
    private async Task<bool> Existe(int estadoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProductos.AnyAsync(e => e.EstadoProductoId == estadoId);
    }
    private async Task<bool> Insertar(EstadosProductos estado)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.EstadosProductos.Add(estado);
        return await contexto.SaveChangesAsync() > 0;
    }
    private async Task<bool> Modificar(EstadosProductos estado)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(estado);
        return await contexto.SaveChangesAsync() > 0;
    }
    public async Task<EstadosProductos?> Buscar(int estadoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProductos.FirstOrDefaultAsync(e => e.EstadoProductoId == estadoId);
    }
    public async Task<bool> Eliminar(int estadoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProductos.AsNoTracking().Where(e => e.EstadoProductoId == estadoId).ExecuteDeleteAsync() > 0;

    }
    public async Task<List<EstadosProductos>> Listar(Expression<Func<EstadosProductos, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProductos.Where(criterio).AsNoTracking().ToListAsync();
    }
}