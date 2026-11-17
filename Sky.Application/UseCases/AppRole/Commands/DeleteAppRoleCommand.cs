namespace Sky.Application.UseCases.AppRole.Commands;

using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;


public sealed record DeleteAppRoleCommand(int AppRoleId) : IRequest<bool>;

internal sealed class DeleteAppRoleHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAppRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteAppRoleCommand request, CancellationToken ct)
    {
        var appRole = await uow.Repository<AppRoleEntity>().GetByIdAsync(request.AppRoleId, ct);
        if (appRole is null) return false;

        uow.Repository<AppRoleEntity>().Delete(appRole);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}