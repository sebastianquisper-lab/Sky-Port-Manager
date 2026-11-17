namespace Sky.Application.UseCases.AppRole.Queries;

using MediatR;
using Sky.Domain.Ports;
using AppRoleEntity = Sky.Domain.Entities.AppRole;


public sealed record GetAppRoleByIdQuery(int AppRoleId) : IRequest<AppRoleEntity?>;

internal sealed class GetAppRoleByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAppRoleByIdQuery, AppRoleEntity?>
{
    public Task<AppRoleEntity?> Handle(GetAppRoleByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AppRoleEntity>().GetByIdAsync(request.AppRoleId, ct);
    }
}