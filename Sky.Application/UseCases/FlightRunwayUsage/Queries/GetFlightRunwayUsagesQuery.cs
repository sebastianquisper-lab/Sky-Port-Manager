using MediatR;
using Sky.Domain.Ports;
using FlightRunwayUsageEntity = Sky.Domain.Entities.FlightRunwayUsage;

namespace Sky.Application.UseCases.FlightRunwayUsage.Queries;

public sealed record GetFlightRunwayUsagesQuery : IRequest<IReadOnlyList<FlightRunwayUsageEntity>>;

internal sealed class GetFlightRunwayUsagesHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightRunwayUsagesQuery, IReadOnlyList<FlightRunwayUsageEntity>>
{
    public Task<IReadOnlyList<FlightRunwayUsageEntity>> Handle(GetFlightRunwayUsagesQuery request, CancellationToken ct)
        => uow.Repository<FlightRunwayUsageEntity>().GetAllAsync(ct);
}
