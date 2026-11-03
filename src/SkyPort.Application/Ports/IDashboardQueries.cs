using SkyPort.Application.UseCases.Dashboard.Dtos;

namespace SkyPort.Application.Ports;

public interface IDashboardQueries
{
    Task<DashboardDto> GetResumenAsync(CancellationToken cancellationToken = default);
}
