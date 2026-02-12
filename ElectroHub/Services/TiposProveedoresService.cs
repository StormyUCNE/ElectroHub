using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroHub.Services;

public class TiposProveedoresService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Guardar(TiposProveedores TiposProveedores)
    {
        if (!await Existe(TiposProveedores.TipoProveedorId))
            return await Insertar(TiposProveedores);
        else
            return await Modificar(TiposProveedores);
    }

    private async Task<bool> Existe(int tipoProveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.TiposProveedores.AnyAsync(p => p.TipoProveedorId == tipoProveedorId);
    }

    private async Task<bool> Insertar(TiposProveedores tiposProveedores)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.TiposProveedores.Add(tiposProveedores);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(TiposProveedores tiposProveedores)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(tiposProveedores);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<TiposProveedores?> Buscar(int tipoProveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.TiposProveedores.FirstOrDefaultAsync(p => p.TipoProveedorId == tipoProveedorId);
    }


    public async Task<bool> Eliminar(int tipoProveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.TiposProveedores.AsNoTracking().Where(p => p.TipoProveedorId == tipoProveedorId).ExecuteDeleteAsync() > 0;
    }


    public async Task<List<TiposProveedores>> Listar(Expression<Func<TiposProveedores, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.TiposProveedores.Where(criterio).AsNoTracking().ToListAsync();
    }

}