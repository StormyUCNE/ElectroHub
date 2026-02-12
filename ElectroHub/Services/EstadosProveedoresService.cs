using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace ElectroHub.Services;

public class EstadosProveedoresService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Guardar(EstadosProveedores estadosProveedores)
    {
        if (await Existe(estadosProveedores.EstadoProveedorId))
            return false;
        if (!await Existe(estadosProveedores.EstadoProveedorId))
            return await Insertar(estadosProveedores);
        else
            return await Modificar(estadosProveedores);
    }
    private async Task<bool> Existe(int estadoProveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProveedores.AnyAsync(p => p.EstadoProveedorId == estadoProveedorId);
    }
    private async Task<bool> Insertar(EstadosProveedores estadosProveedores)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.EstadosProveedores.Add(estadosProveedores);
        return await contexto.SaveChangesAsync() > 0;
    }
    private async Task<bool> Modificar(EstadosProveedores estadosProveedores)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(estadosProveedores);
        return await contexto.SaveChangesAsync() > 0;
    }
    public async Task<EstadosProveedores?> Buscar(int estadoProveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProveedores.FirstOrDefaultAsync(p => p.EstadoProveedorId == estadoProveedorId);
    }
    public async Task<bool> Eliminar(int estadoProveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProveedores.AsNoTracking().Where(p => p.EstadoProveedorId == estadoProveedorId).ExecuteDeleteAsync() > 0;
    }
    public async Task<List<EstadosProveedores>> Listar(Expression<Func<EstadosProveedores, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.EstadosProveedores.Where(criterio).AsNoTracking().ToListAsync();
    }
}