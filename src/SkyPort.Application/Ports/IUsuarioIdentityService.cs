using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;

namespace SkyPort.Application.Ports;

public interface IUsuarioIdentityService
{
    Task<PagedResult<UsuarioDto>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken);
}
