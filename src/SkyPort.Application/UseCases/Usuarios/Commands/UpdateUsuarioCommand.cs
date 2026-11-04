using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record UpdateUsuarioCommand(string Id, string Email, string UserName) : IRequest<Result>;

public sealed class UpdateUsuarioCommandHandler(IUsuarioIdentityService users)
    : IRequestHandler<UpdateUsuarioCommand, Result>
{
    public Task<Result> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken) =>
        users.UpdateProfileAsync(request.Id, request.Email.Trim(), request.UserName.Trim(), cancellationToken);
}
