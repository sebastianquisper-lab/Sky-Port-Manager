using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;

namespace Sky.Application.UseCases.AppRole.Commands;

public sealed record UpdateAppRoleCommand(
    int AppRoleId,
    string RoleName,
    string? Description) : IRequest<AppRoleEntity?>;

internal sealed class UpdateAppRoleHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAppRoleCommand, AppRoleEntity?>
{
    public async Task<AppRoleEntity?> Handle(UpdateAppRoleCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AppRoleEntity>().GetByIdAsync(request.AppRoleId, ct);
        if (entity is null) return null;

        entity.RoleName = request.RoleName;
        entity.Description = request.Description;

        uow.Repository<AppRoleEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
