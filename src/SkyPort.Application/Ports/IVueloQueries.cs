using SkyPort.Application.Dtos.Vuelos;

namespace SkyPort.Application.Ports;

public interface IVueloQueries
{
    Task<IReadOnlyList<VueloListItemDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<VueloDetailDto?> GetByIdAsync(int vueloId, CancellationToken cancellationToken);

    Task<VueloLookupsDto> GetCreateOptionsAsync(CancellationToken cancellationToken);

    Task<bool> HasReservationsOrCrewAsync(int vueloId, CancellationToken cancellationToken);
}
