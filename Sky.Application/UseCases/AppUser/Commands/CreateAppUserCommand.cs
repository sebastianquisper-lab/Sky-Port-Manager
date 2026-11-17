using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;

namespace Sky.Application.UseCases.AppUser.Commands;

/// <summary>
/// La unicidad de username y email (sin distinguir mayúsculas) la garantizan los índices
/// uq_user_username y uq_user_email de PostgreSQL.
/// </summary>
public sealed record CreateAppUserCommand(
    int AppRoleId,
    int? EmployeeId,
    string Username,
    string Email,
    string Password) : IRequest<AppUserEntity>;

internal sealed class CreateAppUserHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    IPasswordHasher hasher)
    : IRequestHandler<CreateAppUserCommand, AppUserEntity>
{
    public async Task<AppUserEntity> Handle(CreateAppUserCommand request, CancellationToken ct)
    {
        var entity = new AppUserEntity
        {
            AppRoleId = request.AppRoleId,
            EmployeeId = request.EmployeeId,
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = hasher.Hash(request.Password),
            IsActive = true,
            CreatedAtUtc = clock.UtcNow
        };

        await uow.Repository<AppUserEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
