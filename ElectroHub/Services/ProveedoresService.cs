using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroHub.Services;

public class ProveedoresService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Guardar(Proveedores Proveedores)
    {
        if (await Existe(Proveedores.ProveedorId))
            return false;
        if (!await Existe(Proveedores.ProveedorId))
            return await Insertar(Proveedores);
        else
            return await Modificar(Proveedores);
    }

    private async Task<bool> Existe(int proveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Proveedores.AnyAsync(p => p.ProveedorId == proveedorId);
    }

    private async Task<bool> Insertar(Proveedores proveedor)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Proveedores.Add(proveedor);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Proveedores proveedor)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(proveedor);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Proveedores?> Buscar(int proveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Proveedores.FirstOrDefaultAsync(p => p.ProveedorId == proveedorId);
    }


    public async Task<bool> Eliminar(int proveedorId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Proveedores?.AsNoTracking().Where(p => p.ProveedorId == proveedorId).ExecuteUpdateAsync(p => p.SetProperty(p => p.Eliminado, true));
        return await contexto.SaveChangesAsync() > 0;
    }


    public async Task<List<Proveedores>> Listar(Expression<Func<Proveedores, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Proveedores.Include(e => e.EstadosProveedores).Include(t => t.TiposProveedores).Where(criterio).ToListAsync();
    }

}