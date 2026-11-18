using MediatR;
using Sky.Domain.Ports;
using FlightDelayEntity = Sky.Domain.Entities.FlightDelay;

namespace Sky.Application.UseCases.FlightDelay.Queries;

public sealed record GetFlightDelaysQuery : IRequest<IReadOnlyList<FlightDelayEntity>>;

internal sealed class GetFlightDelaysHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightDelaysQuery, IReadOnlyList<FlightDelayEntity>>
{
    public Task<IReadOnlyList<FlightDelayEntity>> Handle(GetFlightDelaysQuery request, CancellationToken ct)
        => uow.Repository<FlightDelayEntity>().GetAllAsync(ct);
}
