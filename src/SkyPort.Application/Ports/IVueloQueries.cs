using SkyPort.Application.Dtos.Vuelos;

namespace SkyPort.Application.Ports;

public interface IVueloQueries
{
    Task<IReadOnlyList<VueloListItemDto>> GetAllAsync(CancellationToken cancellationToken);
}
