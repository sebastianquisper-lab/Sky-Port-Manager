using MediatR;
using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Roles.Queries;

public sealed record GetRolesQuery : IRequest<IReadOnlyList<RolDto>>;

public sealed class GetRolesQueryHandler(IRolIdentityService roles)
    : IRequestHandler<GetRolesQuery, IReadOnlyList<RolDto>>
{
    public Task<IReadOnlyList<RolDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken) =>
        roles.GetAllAsync(cancellationToken);
}
