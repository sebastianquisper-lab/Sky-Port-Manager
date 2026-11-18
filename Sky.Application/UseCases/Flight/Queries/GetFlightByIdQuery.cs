using MediatR;
using Sky.Domain.Ports;
using FlightEntity = Sky.Domain.Entities.Flight;

namespace Sky.Application.UseCases.Flight.Queries;

public sealed record GetFlightByIdQuery(
    int FlightId) : IRequest<FlightEntity?>;

internal sealed class GetFlightByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightByIdQuery, FlightEntity?>
{
    public Task<FlightEntity?> Handle(GetFlightByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightEntity>().GetByIdAsync(request.FlightId, ct);
}
