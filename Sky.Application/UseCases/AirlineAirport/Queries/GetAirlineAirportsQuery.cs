namespace Sky.Application.UseCases.AirlineAirport.Queries;

using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;


public sealed record GetAirlineAirportsQuery : IRequest<List<AirlineAirportEntity>>;

internal sealed class GetAirlineAirportsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirlineAirportsQuery, List<AirlineAirportEntity>>
{
    public Task<List<AirlineAirportEntity>> Handle(GetAirlineAirportsQuery request, CancellationToken ct)
    {
        return uow.Repository<AirlineAirportEntity>().GetAllAsync(ct);
    }
}