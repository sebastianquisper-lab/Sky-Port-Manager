using MediatR;
using Sky.Domain.Ports;
using FlightDelayEntity = Sky.Domain.Entities.FlightDelay;

namespace Sky.Application.UseCases.FlightDelay.Queries;

public sealed record GetFlightDelayByIdQuery(
    int FlightDelayId) : IRequest<FlightDelayEntity?>;

internal sealed class GetFlightDelayByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightDelayByIdQuery, FlightDelayEntity?>
{
    public Task<FlightDelayEntity?> Handle(GetFlightDelayByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightDelayEntity>().GetByIdAsync(request.FlightDelayId, ct);
}
