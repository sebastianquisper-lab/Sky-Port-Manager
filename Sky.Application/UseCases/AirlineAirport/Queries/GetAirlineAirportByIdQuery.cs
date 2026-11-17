using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;

namespace Sky.Application.UseCases.AirlineAirport.Queries;

public sealed record GetAirlineAirportByIdQuery(
    int AirlineId,
    int AirportId) : IRequest<AirlineAirportEntity?>;

internal sealed class GetAirlineAirportByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirlineAirportByIdQuery, AirlineAirportEntity?>
{
    public Task<AirlineAirportEntity?> Handle(GetAirlineAirportByIdQuery request, CancellationToken ct)
        => uow.Repository<AirlineAirportEntity>().FirstOrDefaultAsync(x => x.AirlineId == request.AirlineId && x.AirportId == request.AirportId, ct);
}
