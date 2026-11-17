using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;

namespace Sky.Application.UseCases.AppUser.Commands;

public sealed record DeleteAppUserCommand(
    int UserId) : IRequest<bool>;

internal sealed class DeleteAppUserHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAppUserCommand, bool>
{
    public async Task<bool> Handle(DeleteAppUserCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AppUserEntity>().GetByIdAsync(request.UserId, ct);
        if (entity is null) return false;

        uow.Repository<AppUserEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
