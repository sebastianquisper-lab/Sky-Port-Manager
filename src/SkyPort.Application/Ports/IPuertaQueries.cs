using SkyPort.Application.Dtos.Puertas;

namespace SkyPort.Application.Ports;

public interface IPuertaQueries
{
    Task<IReadOnlyList<PuertaDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PuertaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
