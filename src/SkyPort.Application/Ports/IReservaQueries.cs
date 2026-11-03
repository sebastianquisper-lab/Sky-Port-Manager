using SkyPort.Application.UseCases.Reservas.Dtos;

namespace SkyPort.Application.Ports;

public interface IReservaQueries
{
    Task<IReadOnlyList<VueloAbiertoDto>> GetVuelosAbiertosAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReservaDto>> GetByVueloAsync(int vueloId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReservaDto>> GetByPasajeroAsync(int pasajeroId, CancellationToken cancellationToken = default);

    Task<AsientosVueloDto?> GetAsientosAsync(int vueloId, CancellationToken cancellationToken = default);
}
