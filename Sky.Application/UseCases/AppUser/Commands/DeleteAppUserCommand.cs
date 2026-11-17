namespace Sky.Application.UseCases.AppUser.Commands;

using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;


public sealed record DeleteAppUserCommand(int UserId) : IRequest<bool>;

internal sealed class DeleteAppUserHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAppUserCommand, bool>
{
    public async Task<bool> Handle(DeleteAppUserCommand request, CancellationToken ct)
    {
        var appUser = await uow.Repository<AppUserEntity>().GetByIdAsync(request.UserId, ct);
        if (appUser is null) return false;

        uow.Repository<AppUserEntity>().Delete(appUser);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}