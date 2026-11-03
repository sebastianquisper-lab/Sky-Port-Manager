using MediatR;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Dashboard.Dtos;

namespace SkyPort.Application.UseCases.Dashboard;

public sealed class GetDashboardQueryHandler(IDashboardQueries dashboardQueries)
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    public Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken) =>
        dashboardQueries.GetResumenAsync(cancellationToken);
}
