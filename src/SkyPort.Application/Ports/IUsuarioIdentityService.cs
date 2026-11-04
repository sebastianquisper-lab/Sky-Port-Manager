using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;

namespace SkyPort.Application.Ports;

public interface IUsuarioIdentityService
{
    Task<PagedResult<UsuarioDto>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<UsuarioDetalleDto?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<Result<UsuarioDetalleDto>> CreateAsync(string email, string password, CancellationToken cancellationToken);

    Task<Result> UpdateProfileAsync(string id, string email, string userName, CancellationToken cancellationToken);
}
