namespace Sky.Application.UseCases.AppUser.Commands;

using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;


public sealed record UpdateAppUserCommand(
    int UserId,
    int AppRoleId,
    int? EmployeeId,
    string Username,
    string Email,
    string PasswordHash,
    bool IsActive,
    DateTime? LastLoginUtc) : IRequest<AppUserEntity?>;

internal sealed class UpdateAppUserHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAppUserCommand, AppUserEntity?>
{
    public async Task<AppUserEntity?> Handle(UpdateAppUserCommand request, CancellationToken ct)
    {
        var appUser = await uow.Repository<AppUserEntity>().GetByIdAsync(request.UserId, ct);
        if (appUser is null) return null;

        appUser.AppRoleId = request.AppRoleId;
        appUser.EmployeeId = request.EmployeeId;
        appUser.Username = request.Username;
        appUser.Email = request.Email;
        appUser.PasswordHash = request.PasswordHash;
        appUser.IsActive = request.IsActive;
        appUser.LastLoginUtc = request.LastLoginUtc;

        uow.Repository<AppUserEntity>().Update(appUser);
        await uow.SaveChangesAsync(ct);

        return appUser;
    }
}