using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Dtos;
using SkyPort.Persistence.Identity;

namespace SkyPort.Infrastructure.Services;

public sealed class UserAccountService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IUserAccountService
{
    private static readonly string[] AllowedRoles =
    [
        Roles.Administrador,
        Roles.OperadorVuelos,
        Roles.AgenteCheckIn
    ];

    public async Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = await userManager.Users.OrderBy(user => user.Email).ToListAsync(cancellationToken);
        var result = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(ToDto(user, roles.FirstOrDefault() ?? "Sin rol"));
        }

        return result;
    }

    public async Task<Result<UserDto>> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result<UserDto>.Failure(Error.NotFound("No se encontró el usuario."));
        }

        var roles = await userManager.GetRolesAsync(user);
        return Result<UserDto>.Success(ToDto(user, roles.FirstOrDefault() ?? "Sin rol"));
    }

    public async Task<Result<string>> CreateAsync(
        string email,
        string nombreCompleto,
        string password,
        string role,
        CancellationToken cancellationToken = default)
    {
        if (!await IsValidRoleAsync(role))
        {
            return Result<string>.Failure(Error.Validation("Selecciona uno de los roles disponibles."));
        }

        var user = new ApplicationUser
        {
            UserName = email.Trim(),
            Email = email.Trim(),
            NombreCompleto = nombreCompleto.Trim()
        };

        var created = await userManager.CreateAsync(user, password);
        if (!created.Succeeded)
        {
            return Result<string>.Failure(MapIdentityErrors(created.Errors));
        }

        var assigned = await userManager.AddToRoleAsync(user, role);
        if (!assigned.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Result<string>.Failure(MapIdentityErrors(assigned.Errors));
        }

        return Result<string>.Success(user.Id);
    }

    public async Task<Result> ChangeRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        if (!await IsValidRoleAsync(role))
        {
            return Result.Failure(Error.Validation("Selecciona uno de los roles disponibles."));
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el usuario."));
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Contains(Roles.Administrador) && role != Roles.Administrador && IsActive(user) &&
            await IsLastActiveAdministratorAsync(user))
        {
            return Result.Failure(Error.Conflict("No se puede quitar el rol Administrador al último administrador activo."));
        }

        var removed = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removed.Succeeded)
        {
            return Result.Failure(MapIdentityErrors(removed.Errors));
        }

        var added = await userManager.AddToRoleAsync(user, role);
        if (!added.Succeeded)
        {
            await userManager.AddToRolesAsync(user, currentRoles);
            return Result.Failure(MapIdentityErrors(added.Errors));
        }

        return Result.Success();
    }

    public async Task<Result> SetActiveAsync(string userId, bool active, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el usuario."));
        }

        if (IsActive(user) == active)
        {
            return Result.Success();
        }

        if (!active && IsActive(user) && await userManager.IsInRoleAsync(user, Roles.Administrador) &&
            await IsLastActiveAdministratorAsync(user))
        {
            return Result.Failure(Error.Conflict("No se puede desactivar al último administrador activo."));
        }

        user.LockoutEnabled = true;
        user.LockoutEnd = active ? null : DateTimeOffset.UtcNow.AddYears(100);
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(MapIdentityErrors(result.Errors));
    }

    private async Task<bool> IsLastActiveAdministratorAsync(ApplicationUser excludedUser)
    {
        var administrators = await userManager.GetUsersInRoleAsync(Roles.Administrador);
        return administrators.Count(user => user.Id != excludedUser.Id && IsActive(user)) == 0;
    }

    private async Task<bool> IsValidRoleAsync(string role) =>
        AllowedRoles.Contains(role, StringComparer.Ordinal) && await roleManager.RoleExistsAsync(role);

    private static UserDto ToDto(ApplicationUser user, string role) =>
        new(user.Id, user.Email ?? string.Empty, user.NombreCompleto, role, IsActive(user));

    private static bool IsActive(ApplicationUser user) =>
        user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow.AddYears(1);

    private static Error MapIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var materialized = errors.ToArray();
        var message = string.Join(" ", materialized.Select(error => error.Description));
        return materialized.Any(error => error.Code is "DuplicateEmail" or "DuplicateUserName")
            ? Error.Conflict("Ya existe una cuenta con ese correo electrónico.")
            : Error.Validation(string.IsNullOrWhiteSpace(message) ? "No se pudo guardar el usuario." : message);
    }
}
