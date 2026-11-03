using MediatR;
using SkyPort.Application.UseCases.Dashboard.Dtos;

namespace SkyPort.Application.UseCases.Dashboard;

public sealed record GetDashboardQuery : IRequest<DashboardDto>;
