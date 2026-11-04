using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Boletos;

namespace SkyPort.Application.Ports;

public interface IBoletoQueries
{
    Task<PagedResult<BoletoDto>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<BoletoDto?> GetByIdAsync(int boletoId, CancellationToken cancellationToken = default);
}
