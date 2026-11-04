using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Roles.Queries;

public sealed record GetRolByIdQuery(string Id) : IRequest<Result<RolDetalleDto>>;

public sealed class GetRolByIdQueryHandler(IRolIdentityService roles)
    : IRequestHandler<GetRolByIdQuery, Result<RolDetalleDto>>
{
    public async Task<Result<RolDetalleDto>> Handle(GetRolByIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            return Result<RolDetalleDto>.Failure(Error.NotFound("No se encontró el rol."));
        }

        var role = await roles.GetByIdAsync(request.Id, cancellationToken);
        return role is null
            ? Result<RolDetalleDto>.Failure(Error.NotFound("No se encontró el rol."))
            : Result<RolDetalleDto>.Success(role);
    }
}
