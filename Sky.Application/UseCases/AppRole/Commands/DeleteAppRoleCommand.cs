using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;

namespace Sky.Application.UseCases.AppRole.Commands;

public sealed record DeleteAppRoleCommand(
    int AppRoleId) : IRequest<bool>;

internal sealed class DeleteAppRoleHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAppRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteAppRoleCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AppRoleEntity>().GetByIdAsync(request.AppRoleId, ct);
        if (entity is null) return false;

        uow.Repository<AppRoleEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
