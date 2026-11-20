using MediatR;
using Sky.Domain.Ports;
using FlightStatusHistoryEntity = Sky.Domain.Entities.FlightStatusHistory;

namespace Sky.Application.UseCases.FlightStatusHistory.Queries;

public sealed record GetFlightStatusHistoriesQuery : IRequest<IReadOnlyList<FlightStatusHistoryEntity>>;

internal sealed class GetFlightStatusHistoriesHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightStatusHistoriesQuery, IReadOnlyList<FlightStatusHistoryEntity>>
{
    public Task<IReadOnlyList<FlightStatusHistoryEntity>> Handle(GetFlightStatusHistoriesQuery request, CancellationToken ct)
        => uow.Repository<FlightStatusHistoryEntity>().GetAllAsync(ct);
}
