using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Ports;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;

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

    public async Task<Result<RolDto>> CreateAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<RolDto>.Failure(Error.Validation("El nombre del rol es obligatorio."));
        }

        if (IsReservedName(name))
        {
            return Result<RolDto>.Failure(Error.Conflict("Ese nombre está reservado por las políticas y el seeder."));
        }

        var role = new IdentityRole(name);
        var result = await roleManager.CreateAsync(role);
        cancellationToken.ThrowIfCancellationRequested();
        if (!result.Succeeded)
        {
            var codes = result.Errors.Select(error => error.Code).ToHashSet(StringComparer.Ordinal);
            return codes.Contains("DuplicateRoleName")
                ? Result<RolDto>.Failure(Error.Conflict("Ya existe un rol con ese nombre."))
                : Result<RolDto>.Failure(Error.Validation("Identity rechazó el nombre del rol."));
        }

        return Result<RolDto>.Success(new RolDto(role.Id, role.Name ?? name));
    }

    public async Task<Result> RenameAsync(string id, string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el rol."));
        }

        if (IsReservedName(role.Name) || IsReservedName(name))
        {
            return Result.Failure(Error.Conflict("Los roles usados por las políticas y el seeder no se pueden renombrar."));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(Error.Validation("El nombre del rol es obligatorio."));
        }

        var users = await userManager.GetUsersInRoleAsync(role.Name!);
        if (users.Count > 0)
        {
            return Result.Failure(Error.Conflict("No se puede renombrar un rol que ya tiene miembros."));
        }

        role.Name = name;
        var result = await roleManager.UpdateAsync(role);
        cancellationToken.ThrowIfCancellationRequested();
        if (result.Succeeded)
        {
            return Result.Success();
        }

        var codes = result.Errors.Select(error => error.Code).ToHashSet(StringComparer.Ordinal);
        if (codes.Contains("DuplicateRoleName"))
        {
            return Result.Failure(Error.Conflict("Ya existe un rol con ese nombre."));
        }

        if (codes.Contains("ConcurrencyFailure"))
        {
            return Result.Failure(Error.Conflict("El rol cambió desde que se abrió el formulario. Recarga los datos e inténtalo nuevamente."));
        }

        return Result.Failure(Error.Validation("Identity rechazó el nombre del rol."));
    }

    private static bool IsReservedName(string? name) =>
        string.Equals(name, Roles.Administrador, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(name, Roles.OperadorVuelos, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(name, Roles.AgenteCheckIn, StringComparison.OrdinalIgnoreCase);
}
