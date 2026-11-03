using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Pasajeros.Dtos;

namespace SkyPort.Application.Ports;

public interface IPasajeroQueries
{
    Task<PagedResult<PasajeroDto>> GetPageAsync(
        string? busqueda,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PasajeroDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PasajeroDto?> GetByDocumentoAsync(
        string tipoDocumento,
        string numeroDocumento,
        CancellationToken cancellationToken = default);
}
