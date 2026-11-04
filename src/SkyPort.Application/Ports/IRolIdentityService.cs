using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Common;

namespace SkyPort.Application.Ports;

public interface IRolIdentityService
{
    Task<IReadOnlyList<RolDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<RolDetalleDto?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<Result<RolDto>> CreateAsync(string name, CancellationToken cancellationToken);

    Task<Result> RenameAsync(string id, string name, CancellationToken cancellationToken);
}
