using SkyPort.Application.Dtos.Equipajes;

namespace SkyPort.Application.Ports;

public interface IEquipajeQueries
{
    Task<IReadOnlyList<EquipajeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EquipajeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
