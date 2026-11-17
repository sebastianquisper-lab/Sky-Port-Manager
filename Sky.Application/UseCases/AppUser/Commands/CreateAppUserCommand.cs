namespace Sky.Application.UseCases.AppUser.Commands;

using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;


public sealed record CreateAppUserCommand(
    int AppRoleId,
    int? EmployeeId,
    string Username,
    string Email,
    string PasswordHash,
    DateTime? LastLoginUtc) : IRequest<AppUserEntity>;

internal sealed class CreateAppUserHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAppUserCommand, AppUserEntity>
{
    public async Task<AppUserEntity> Handle(CreateAppUserCommand request, CancellationToken ct)
    {
        var appUser = new AppUserEntity
        {
            AppRoleId = request.AppRoleId,
            EmployeeId = request.EmployeeId,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.PasswordHash,
            IsActive = true,
            LastLoginUtc = request.LastLoginUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        await uow.Repository<AppUserEntity>().AddAsync(appUser, ct);
        await uow.SaveChangesAsync(ct);

        return appUser;
    }
}