using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;

namespace Sky.Application.UseCases.AppUser.Commands;

/// <param name="NewPassword">Si viene null o vacío, la contraseña no se modifica.</param>
public sealed record UpdateAppUserCommand(
    int UserId,
    int AppRoleId,
    int? EmployeeId,
    string Username,
    string Email,
    bool IsActive,
    string? NewPassword) : IRequest<AppUserEntity?>;

internal sealed class UpdateAppUserHandler(IUnitOfWork uow, IPasswordHasher hasher)
    : IRequestHandler<UpdateAppUserCommand, AppUserEntity?>
{
    public async Task<AppUserEntity?> Handle(UpdateAppUserCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AppUserEntity>().GetByIdAsync(request.UserId, ct);
        if (entity is null) return null;

        entity.AppRoleId = request.AppRoleId;
        entity.EmployeeId = request.EmployeeId;
        entity.Username = request.Username.Trim();
        entity.Email = request.Email.Trim();
        entity.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            entity.PasswordHash = hasher.Hash(request.NewPassword);
        }

        uow.Repository<AppUserEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
