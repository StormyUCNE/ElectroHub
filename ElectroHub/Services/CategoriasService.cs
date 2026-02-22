using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace ElectroHub.Services;

public class CategoriasService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Guardar(Categorias categoria)
    {
        if (!await Existe(categoria.CategoriaId))
            return await Insertar(categoria);
        else
            return await Modificar(categoria);
    }
    private async Task<bool> Existe(int categoriaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Categorias.AnyAsync(c => c.CategoriaId == categoriaId);
    }
    private async Task<bool> Insertar(Categorias categoria)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Categorias.Add(categoria);
        return await contexto.SaveChangesAsync() > 0;
    }
    private async Task<bool> Modificar(Categorias categoria)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(categoria);
        return await contexto.SaveChangesAsync() > 0;
    }
    public async Task<Categorias?> Buscar(int categoriaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == categoriaId);
    }
    public async Task<bool> Eliminar(int categoriaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var numeroCategoriasAsociadas = await contexto.Productos.CountAsync(p => p.CategoriaId == categoriaId && p.Eliminado == false);
        if (numeroCategoriasAsociadas > 0) return false;
        var filasAfectadas = await contexto.Categorias
        .Where(c => c.CategoriaId == categoriaId)
        .ExecuteUpdateAsync(c => c.SetProperty(p => p.Eliminado, true));
        return filasAfectadas > 0;
    }
    public async Task<List<Categorias>> Listar(Expression<Func<Categorias, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Categorias.Where(criterio).AsNoTracking().ToListAsync();
    }
}
