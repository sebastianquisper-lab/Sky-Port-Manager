namespace Sky.Application.UseCases.AirlineAirport.Queries;

using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;


public sealed record GetAirlineAirportByIdQuery(
    int AirlineId,
    int AirportId) : IRequest<AirlineAirportEntity?>;

internal sealed class GetAirlineAirportByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirlineAirportByIdQuery, AirlineAirportEntity?>
{
    public Task<AirlineAirportEntity?> Handle(GetAirlineAirportByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AirlineAirportEntity>().FirstOrDefaultAsync(x => x.AirlineId == request.AirlineId && x.AirportId == request.AirportId, ct);
    }
}