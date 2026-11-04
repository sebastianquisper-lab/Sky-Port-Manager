using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Queries;

public sealed record GetUsuarioRolesQuery(string UserId) : IRequest<Result<IReadOnlyList<UsuarioRolMembershipDto>>>;

public sealed class GetUsuarioRolesQueryHandler(IUsuarioRolService memberships)
    : IRequestHandler<GetUsuarioRolesQuery, Result<IReadOnlyList<UsuarioRolMembershipDto>>>
{
    public async Task<Result<IReadOnlyList<UsuarioRolMembershipDto>>> Handle(
        GetUsuarioRolesQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return Result<IReadOnlyList<UsuarioRolMembershipDto>>.Failure(Error.NotFound("No se encontró el usuario."));
        }

        var result = await memberships.GetMembershipsAsync(request.UserId, cancellationToken);
        return result is null
            ? Result<IReadOnlyList<UsuarioRolMembershipDto>>.Failure(Error.NotFound("No se encontró el usuario."))
            : Result<IReadOnlyList<UsuarioRolMembershipDto>>.Success(result);
    }
}
