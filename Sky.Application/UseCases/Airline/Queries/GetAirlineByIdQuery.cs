using MediatR;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;

namespace Sky.Application.UseCases.Airline.Queries;

public sealed record GetAirlineByIdQuery(
    int AirlineId) : IRequest<AirlineEntity?>;

internal sealed class GetAirlineByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirlineByIdQuery, AirlineEntity?>
{
    public Task<AirlineEntity?> Handle(GetAirlineByIdQuery request, CancellationToken ct)
        => uow.Repository<AirlineEntity>().GetByIdAsync(request.AirlineId, ct);
}
