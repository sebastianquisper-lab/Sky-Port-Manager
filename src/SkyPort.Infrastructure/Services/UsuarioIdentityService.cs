using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Identity;

namespace SkyPort.Infrastructure.Services;

public sealed class UsuarioIdentityService(UserManager<ApplicationUser> userManager) : IUsuarioIdentityService
{
    public async Task<PagedResult<UsuarioDto>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var users = userManager.Users.AsNoTracking();
        var totalCount = await users.CountAsync(cancellationToken);
        var items = await users
            .OrderBy(user => user.Email)
            .ThenBy(user => user.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new UsuarioDto(user.Id, user.Email, user.UserName, user.EmailConfirmed))
            .ToListAsync(cancellationToken);

        return new PagedResult<UsuarioDto>(items, page, pageSize, totalCount);
    }

    public async Task<UsuarioDetalleDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var roles = await userManager.GetRolesAsync(user);
        return new UsuarioDetalleDto(user.Id, user.Email, user.UserName, user.EmailConfirmed, roles.Order(StringComparer.Ordinal).ToArray());
    }

    public async Task<Result<UsuarioDetalleDto>> CreateAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = new ApplicationUser { Email = email, UserName = email };
        var result = await userManager.CreateAsync(user, password);
        cancellationToken.ThrowIfCancellationRequested();
        if (!result.Succeeded)
        {
            var codes = result.Errors.Select(error => error.Code).ToHashSet(StringComparer.Ordinal);
            if (codes.Contains("DuplicateEmail") || codes.Contains("DuplicateUserName"))
            {
                return Result<UsuarioDetalleDto>.Failure(Error.Conflict("Ya existe una cuenta con ese correo."));
            }

            if (codes.Any(code => code.StartsWith("Password", StringComparison.Ordinal)))
            {
                return Result<UsuarioDetalleDto>.Failure(Error.Validation("La contraseña no cumple los requisitos configurados."));
            }

            return Result<UsuarioDetalleDto>.Failure(Error.Validation("Identity rechazó los datos de la cuenta."));
        }

        return Result<UsuarioDetalleDto>.Success(new UsuarioDetalleDto(user.Id, user.Email, user.UserName, user.EmailConfirmed, Array.Empty<string>()));
    }
}
