using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record AsignarUsuarioRolCommand(string UserId, string RoleId) : IRequest<Result>;

public sealed class AsignarUsuarioRolCommandHandler(IUsuarioRolService memberships)
    : IRequestHandler<AsignarUsuarioRolCommand, Result>
{
    public Task<Result> Handle(AsignarUsuarioRolCommand request, CancellationToken cancellationToken) =>
        memberships.AssignAsync(request.UserId, request.RoleId, cancellationToken);
}
