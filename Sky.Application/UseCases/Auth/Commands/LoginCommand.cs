using MediatR;
using Sky.Application.DTOs;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;
using AppUserEntity = Sky.Domain.Entities.AppUser;

namespace Sky.Application.UseCases.Auth.Commands;

public sealed record LoginCommand(string Username, string Password) : IRequest<LoginResponseDto>;

internal sealed class LoginHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    IPasswordHasher hasher,
    IJwtTokenGenerator jwt)
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var username = request.Username.Trim().ToLower();

        var appUser = await uow.Repository<AppUserEntity>()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username, ct);

        // Mismo mensaje para usuario inexistente, inactivo o contraseña incorrecta.
        if (appUser is null || !appUser.IsActive || !hasher.Verify(request.Password, appUser.PasswordHash))
        {
            throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
        }

        var role = await uow.Repository<AppRoleEntity>().GetByIdAsync(appUser.AppRoleId, ct)
            ?? throw new KeyNotFoundException($"El rol {appUser.AppRoleId} no existe.");

        var (token, expiresAtUtc) = jwt.Generate(appUser, role);

        appUser.LastLoginUtc = clock.UtcNow;
        uow.Repository<AppUserEntity>().Update(appUser);
        await uow.SaveChangesAsync(ct);

        return new LoginResponseDto(
            token,
            expiresAtUtc,
            appUser.UserId,
            appUser.Username,
            role.RoleName,
            appUser.EmployeeId);
    }
}
