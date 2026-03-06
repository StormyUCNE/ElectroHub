using ElectroHub.Data;
using ElectroHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroHub.Services;

public class AdministradorCuentasService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<List<UsuariosRoles>> Listar()
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var query = from user in contexto.Users
                    join userRole in contexto.UserRoles
                        on user.Id equals userRole.UserId
                    join role in contexto.Roles
                        on userRole.RoleId equals role.Id
                    select new UsuariosRoles
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Rol = role.Name
                    };
        return await query.AsNoTracking().ToListAsync();
    }
    public async Task<List<IdentityRole>> ListarRoles()
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        return await contexto.Roles
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task CambiarRol(string userId, string nuevoRol)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var role = await contexto.Roles
            .FirstOrDefaultAsync(r => r.Name == nuevoRol);
        if (role == null)
            return;
        var rolesActuales = await contexto.UserRoles
            .Where(x => x.UserId == userId)
            .ToListAsync();

        contexto.UserRoles.RemoveRange(rolesActuales);
        contexto.UserRoles.Add(new IdentityUserRole<string>
        {
            UserId = userId,
            RoleId = role.Id
        });
        await contexto.SaveChangesAsync();
    }
}

