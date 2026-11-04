using SkyPort.Application.Common;
using SkyPort.Application.Dtos.CheckIn;

namespace SkyPort.Application.Ports;

public interface ICheckInQueries
{
    Task<PagedResult<CheckInDto>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<CheckInDto?> GetByIdAsync(int checkInId, CancellationToken cancellationToken = default);
}
