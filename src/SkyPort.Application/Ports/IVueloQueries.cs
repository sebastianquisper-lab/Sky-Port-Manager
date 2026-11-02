using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Vuelos.Dtos;

namespace SkyPort.Application.Ports;

public interface IVueloQueries
{
    Task<PagedResult<VueloListItemDto>> SearchAsync(
        VueloFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<VueloDetailDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);

    Task<VueloLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default);
}
