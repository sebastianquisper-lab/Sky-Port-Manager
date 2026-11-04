using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Ports;

namespace SkyPort.Infrastructure.Services;

public sealed class RolIdentityService(
    RoleManager<IdentityRole> roleManager,
    UserManager<Persistence.Identity.ApplicationUser> userManager) : IRolIdentityService
{
    public async Task<IReadOnlyList<RolDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await roleManager.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .ThenBy(role => role.Id)
            .Select(role => new RolDto(role.Id, role.Name ?? string.Empty))
            .ToListAsync(cancellationToken);
    }

    public async Task<RolDetalleDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return null;
        }

        var users = await userManager.GetUsersInRoleAsync(role.Name!);
        cancellationToken.ThrowIfCancellationRequested();
        return new RolDetalleDto(
            role.Id,
            role.Name ?? string.Empty,
            users.Select(user => user.Email ?? user.UserName ?? user.Id).Order(StringComparer.OrdinalIgnoreCase).ToArray());
    }
}
