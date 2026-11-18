using MediatR;
using Sky.Domain.Ports;
using FlightRunwayUsageEntity = Sky.Domain.Entities.FlightRunwayUsage;

namespace Sky.Application.UseCases.FlightRunwayUsage.Queries;

public sealed record GetFlightRunwayUsageByIdQuery(
    int UsageId) : IRequest<FlightRunwayUsageEntity?>;

internal sealed class GetFlightRunwayUsageByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightRunwayUsageByIdQuery, FlightRunwayUsageEntity?>
{
    public Task<FlightRunwayUsageEntity?> Handle(GetFlightRunwayUsageByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightRunwayUsageEntity>().GetByIdAsync(request.UsageId, ct);
}
