using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;

namespace Sky.Application.UseCases.AppRole.Queries;

public sealed record GetAppRolesQuery : IRequest<IReadOnlyList<AppRoleEntity>>;

internal sealed class GetAppRolesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAppRolesQuery, IReadOnlyList<AppRoleEntity>>
{
    public Task<IReadOnlyList<AppRoleEntity>> Handle(GetAppRolesQuery request, CancellationToken ct)
        => uow.Repository<AppRoleEntity>().GetAllAsync(ct);
}
