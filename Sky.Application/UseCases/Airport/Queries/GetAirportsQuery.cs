namespace Sky.Application.UseCases.Airport.Queries;

using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;


public sealed record GetAirportsQuery : IRequest<List<AirportEntity>>;

internal sealed class GetAirportsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirportsQuery, List<AirportEntity>>
{
    public Task<List<AirportEntity>> Handle(GetAirportsQuery request, CancellationToken ct)
    {
        return uow.Repository<AirportEntity>().GetAllAsync(ct);
    }
}