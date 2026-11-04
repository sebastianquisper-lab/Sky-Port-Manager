using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Usuarios.Queries;

public sealed record GetUsuariosQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<UsuarioDto>>;

public sealed class GetUsuariosQueryHandler(IUsuarioIdentityService users)
    : IRequestHandler<GetUsuariosQuery, PagedResult<UsuarioDto>>
{
    public Task<PagedResult<UsuarioDto>> Handle(GetUsuariosQuery request, CancellationToken cancellationToken) =>
        users.GetPageAsync(Math.Max(1, request.Page), Math.Clamp(request.PageSize, 1, 100), cancellationToken);
}
