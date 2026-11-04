using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Identity;

namespace SkyPort.Infrastructure.Services;

public sealed class UsuarioRolService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IUsuarioRolService
{
    public async Task<IReadOnlyList<UsuarioRolMembershipDto>?> GetMembershipsAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var roles = await roleManager.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .ThenBy(role => role.Id)
            .ToListAsync(cancellationToken);
        var userRoles = (await userManager.GetRolesAsync(user)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return roles
            .Select(role => new UsuarioRolMembershipDto(
                role.Id,
                role.Name ?? string.Empty,
                role.Name is not null && userRoles.Contains(role.Name)))
            .ToArray();
    }

    public async Task<Result> AssignAsync(string userId, string roleId, CancellationToken cancellationToken)
    {
        var (user, role, failure) = await LoadAsync(userId, roleId, cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        if (await userManager.IsInRoleAsync(user!, role!.Name!))
        {
            return Result.Failure(Error.Conflict("El usuario ya pertenece a ese rol."));
        }

        var stamp = await userManager.UpdateSecurityStampAsync(user!);
        if (!stamp.Succeeded)
        {
            return Result.Failure(Error.Validation("No se pudo actualizar la seguridad de la cuenta."));
        }

        var result = await userManager.AddToRoleAsync(user!, role.Name!);
        cancellationToken.ThrowIfCancellationRequested();
        return MapMembershipResult(result, "No se pudo asignar el rol.");
    }

    public async Task<Result> RevokeAsync(string userId, string roleId, CancellationToken cancellationToken)
    {
        var (user, role, failure) = await LoadAsync(userId, roleId, cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        if (string.Equals(role!.Name, Roles.Administrador, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure(Error.Forbidden("La membresía Administrador está protegida para conservar acceso administrativo."));
        }

        if (!await userManager.IsInRoleAsync(user!, role.Name!))
        {
            return Result.Failure(Error.NotFound("El usuario no pertenece a ese rol."));
        }

        var stamp = await userManager.UpdateSecurityStampAsync(user!);
        if (!stamp.Succeeded)
        {
            return Result.Failure(Error.Validation("No se pudo actualizar la seguridad de la cuenta."));
        }

        var result = await userManager.RemoveFromRoleAsync(user!, role.Name!);
        cancellationToken.ThrowIfCancellationRequested();
        return MapMembershipResult(result, "No se pudo revocar el rol.");
    }

    private async Task<(ApplicationUser? User, IdentityRole? Role, Result? Failure)> LoadAsync(
        string userId,
        string roleId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
        {
            return (null, null, Result.Failure(Error.NotFound("No se encontró el usuario o el rol.")));
        }

        var user = await userManager.FindByIdAsync(userId);
        var role = await roleManager.FindByIdAsync(roleId);
        return user is null || role?.Name is null
            ? (null, null, Result.Failure(Error.NotFound("No se encontró el usuario o el rol.")))
            : (user, role, null);
    }

    private static Result MapMembershipResult(IdentityResult result, string fallback)
    {
        if (result.Succeeded)
        {
            return Result.Success();
        }

        var codes = result.Errors.Select(error => error.Code).ToHashSet(StringComparer.Ordinal);
        if (codes.Contains("UserAlreadyInRole"))
        {
            return Result.Failure(Error.Conflict("El usuario ya pertenece a ese rol."));
        }

        if (codes.Contains("UserNotInRole"))
        {
            return Result.Failure(Error.NotFound("El usuario no pertenece a ese rol."));
        }

        if (codes.Contains("ConcurrencyFailure"))
        {
            return Result.Failure(Error.Conflict("La cuenta cambió desde que se abrió el formulario. Recarga los datos e inténtalo nuevamente."));
        }

        return Result.Failure(Error.Validation(fallback));
    }
}
