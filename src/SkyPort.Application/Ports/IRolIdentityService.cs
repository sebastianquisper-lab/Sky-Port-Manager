using SkyPort.Application.Dtos.Roles;

namespace SkyPort.Application.Ports;

public interface IRolIdentityService
{
    Task<IReadOnlyList<RolDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<RolDetalleDto?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
