using MediatR;
using Sky.Domain.Ports;
using AppUserEntity = Sky.Domain.Entities.AppUser;

namespace Sky.Application.UseCases.AppUser.Queries;

public sealed record GetAppUsersQuery : IRequest<IReadOnlyList<AppUserEntity>>;

internal sealed class GetAppUsersHandler(IUnitOfWork uow)
    : IRequestHandler<GetAppUsersQuery, IReadOnlyList<AppUserEntity>>
{
    public Task<IReadOnlyList<AppUserEntity>> Handle(GetAppUsersQuery request, CancellationToken ct)
        => uow.Repository<AppUserEntity>().GetAllAsync(ct);
}
