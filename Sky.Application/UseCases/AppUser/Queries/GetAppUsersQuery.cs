namespace Sky.Application.UseCases.AppUser.Queries;

using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;


public sealed record GetAppUsersQuery : IRequest<List<AppUserEntity>>;

internal sealed class GetAppUsersHandler(IUnitOfWork uow)
    : IRequestHandler<GetAppUsersQuery, List<AppUserEntity>>
{
    public Task<List<AppUserEntity>> Handle(GetAppUsersQuery request, CancellationToken ct)
    {
        return uow.Repository<AppUserEntity>().GetAllAsync(ct);
    }
}