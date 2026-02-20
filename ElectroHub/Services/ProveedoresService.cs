using ElectroHub.Data;
using Microsoft.EntityFrameworkCore;
using ElectroHub.Models;
using System.Linq.Expressions;

namespace ElectroHub.Services;

public class ProveedoresService(IDbContextFactory<ApplicationDbContext> DbContextFactory)
{
    public async Task<bool> Guardar(Proveedores Provedor)
    {
        if (!await Existe(Provedor.ProveedorId))
            return await Insertar(Provedor);
        else
            return await Modificar(Provedor);
    }
    public async Task<bool> Existe(int Id)
    {

        await using var context = await DbContextFactory.CreateDbContextAsync();
        return await context.Proveedores.AnyAsync(Proveedor => Proveedor.ProveedorId == Id);
    }
    public async Task<bool> Insertar(Proveedores Proveedor)
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();

        context.Proveedores.Add(Proveedor);

        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> Modificar(Proveedores Proveedor)
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();

        context.Update(Proveedor);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Proveedores?> Buscar(int Id)
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();

        return await context.Proveedores.FirstOrDefaultAsync(proveedor => proveedor.ProveedorId == Id);
    }

    public async Task<bool> Eliminar(int Id)
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();
        var filasAfectadas = await context.Proveedores!.Where(proveedor => proveedor.ProveedorId == Id).ExecuteUpdateAsync(proveedor => proveedor.SetProperty(p => p.Eliminado, true));
        return filasAfectadas > 0;
    }

    public async Task<List<Proveedores>> Listar(Expression<Func<Proveedores, bool>> criterio)
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();
        return await context.Proveedores.Where(criterio).ToListAsync();
    }

}