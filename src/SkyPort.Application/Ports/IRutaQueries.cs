using SkyPort.Application.Dtos.Rutas;

namespace SkyPort.Application.Ports;

public interface IRutaQueries
{
    Task<RutaDto?> GetByIdAsync(int rutaId, CancellationToken cancellationToken);
}
