using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Commands;

public sealed record CreateUsuarioCommand(string Email, string Password) : IRequest<Result<UsuarioDetalleDto>>;

public sealed class CreateUsuarioCommandHandler(IUsuarioIdentityService users)
    : IRequestHandler<CreateUsuarioCommand, Result<UsuarioDetalleDto>>
{
    public Task<Result<UsuarioDetalleDto>> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken) =>
        users.CreateAsync(request.Email.Trim(), request.Password, cancellationToken);
}
