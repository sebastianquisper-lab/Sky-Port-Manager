using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Queries;

public sealed record GetUsuarioByIdQuery(string Id) : IRequest<Result<UsuarioDetalleDto>>;

public sealed class GetUsuarioByIdQueryHandler(IUsuarioIdentityService users)
    : IRequestHandler<GetUsuarioByIdQuery, Result<UsuarioDetalleDto>>
{
    public async Task<Result<UsuarioDetalleDto>> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            return Result<UsuarioDetalleDto>.Failure(Error.NotFound("No se encontró el usuario."));
        }

        var usuario = await users.GetByIdAsync(request.Id, cancellationToken);
        return usuario is null
            ? Result<UsuarioDetalleDto>.Failure(Error.NotFound("No se encontró el usuario."))
            : Result<UsuarioDetalleDto>.Success(usuario);
    }
}
