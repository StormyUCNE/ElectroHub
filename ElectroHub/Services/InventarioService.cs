using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroHub.Services;

public class InventarioService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> RegistrarMovimiento(InventarioMovimientos movimiento)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.InventarioMovimientos.Add(movimiento);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<List<InventarioMovimientos>> Listar(Expression<Func<InventarioMovimientos, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        // Incluimos Producto y luego la Categoria de ese producto
        return await contexto.InventarioMovimientos
        .Include(m => m.Producto)
            .ThenInclude(p => p!.Categorias) // Carga la categoría del producto
        .Include(m => m.Producto)
            .ThenInclude(p => p!.Proveedores) // <--- ESTA ES LA LÍNEA QUE FALTABA
        .Where(criterio)
        .OrderByDescending(m => m.FechaMovimiento)
        .AsNoTracking()
        .ToListAsync();
    }
}