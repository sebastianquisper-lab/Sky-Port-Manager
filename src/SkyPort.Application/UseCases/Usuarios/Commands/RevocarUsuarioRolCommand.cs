using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record RevocarUsuarioRolCommand(string UserId, string RoleId) : IRequest<Result>;

public sealed class RevocarUsuarioRolCommandHandler(IUsuarioRolService memberships)
    : IRequestHandler<RevocarUsuarioRolCommand, Result>
{
    public Task<Result> Handle(RevocarUsuarioRolCommand request, CancellationToken cancellationToken) =>
        memberships.RevokeAsync(request.UserId, request.RoleId, cancellationToken);
}
