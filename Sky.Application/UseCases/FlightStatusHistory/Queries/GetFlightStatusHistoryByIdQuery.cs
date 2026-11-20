using MediatR;
using Sky.Domain.Ports;
using FlightStatusHistoryEntity = Sky.Domain.Entities.FlightStatusHistory;

namespace Sky.Application.UseCases.FlightStatusHistory.Queries;

public sealed record GetFlightStatusHistoryByIdQuery(
    int HistoryId) : IRequest<FlightStatusHistoryEntity?>;

internal sealed class GetFlightStatusHistoryByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightStatusHistoryByIdQuery, FlightStatusHistoryEntity?>
{
    public Task<FlightStatusHistoryEntity?> Handle(GetFlightStatusHistoryByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightStatusHistoryEntity>().GetByIdAsync(request.HistoryId, ct);
}
