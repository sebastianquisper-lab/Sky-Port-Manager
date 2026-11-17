using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;

namespace Sky.Application.UseCases.AirlineAirport.Queries;

public sealed record GetAirlineAirportsQuery : IRequest<IReadOnlyList<AirlineAirportEntity>>;

internal sealed class GetAirlineAirportsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirlineAirportsQuery, IReadOnlyList<AirlineAirportEntity>>
{
    public Task<IReadOnlyList<AirlineAirportEntity>> Handle(GetAirlineAirportsQuery request, CancellationToken ct)
        => uow.Repository<AirlineAirportEntity>().GetAllAsync(ct);
}
