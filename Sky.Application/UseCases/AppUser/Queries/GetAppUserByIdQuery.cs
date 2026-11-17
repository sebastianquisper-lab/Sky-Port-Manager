namespace Sky.Application.UseCases.AppUser.Queries;

using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;


public sealed record GetAppUserByIdQuery(int UserId) : IRequest<AppUserEntity?>;

internal sealed class GetAppUserByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAppUserByIdQuery, AppUserEntity?>
{
    public Task<AppUserEntity?> Handle(GetAppUserByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AppUserEntity>().GetByIdAsync(request.UserId, ct);
    }
}