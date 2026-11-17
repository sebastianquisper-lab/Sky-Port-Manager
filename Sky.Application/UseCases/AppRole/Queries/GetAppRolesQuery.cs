namespace Sky.Application.UseCases.AppRole.Queries;

using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;


public sealed record GetAppRolesQuery : IRequest<List<AppRoleEntity>>;

internal sealed class GetAppRolesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAppRolesQuery, List<AppRoleEntity>>
{
    public Task<List<AppRoleEntity>> Handle(GetAppRolesQuery request, CancellationToken ct)
    {
        return uow.Repository<AppRoleEntity>().GetAllAsync(ct);
    }
}