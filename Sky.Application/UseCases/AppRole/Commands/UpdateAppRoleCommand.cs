namespace Sky.Application.UseCases.AppRole.Commands;

using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;


public sealed record UpdateAppRoleCommand(
    int AppRoleId,
    string RoleName,
    string? Description) : IRequest<AppRoleEntity?>;

internal sealed class UpdateAppRoleHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAppRoleCommand, AppRoleEntity?>
{
    public async Task<AppRoleEntity?> Handle(UpdateAppRoleCommand request, CancellationToken ct)
    {
        var appRole = await uow.Repository<AppRoleEntity>().GetByIdAsync(request.AppRoleId, ct);
        if (appRole is null) return null;

        appRole.RoleName = request.RoleName;
        appRole.Description = request.Description;

        uow.Repository<AppRoleEntity>().Update(appRole);
        await uow.SaveChangesAsync(ct);

        return appRole;
    }
}