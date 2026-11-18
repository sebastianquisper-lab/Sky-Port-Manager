using MediatR;
using Sky.Domain.Ports;
using FlightEntity = Sky.Domain.Entities.Flight;

namespace Sky.Application.UseCases.Flight.Queries;

public sealed record GetFlightsQuery : IRequest<IReadOnlyList<FlightEntity>>;

internal sealed class GetFlightsHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightsQuery, IReadOnlyList<FlightEntity>>
{
    public Task<IReadOnlyList<FlightEntity>> Handle(GetFlightsQuery request, CancellationToken ct)
        => uow.Repository<FlightEntity>().GetAllAsync(ct);
}
