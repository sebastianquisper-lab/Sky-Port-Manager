using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record DeleteUsuarioCommand(string Id, string ActorId) : IRequest<Result>;

public sealed class DeleteUsuarioCommandHandler(IUsuarioIdentityService users)
    : IRequestHandler<DeleteUsuarioCommand, Result>
{
    public Task<Result> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken) =>
        users.DeleteAsync(request.Id, request.ActorId, cancellationToken);
}
