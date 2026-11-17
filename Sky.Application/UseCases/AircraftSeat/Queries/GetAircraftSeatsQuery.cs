namespace Sky.Application.UseCases.AircraftSeat.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;


public sealed record GetAircraftSeatsQuery : IRequest<List<AircraftSeatEntity>>;

internal sealed class GetAircraftSeatsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftSeatsQuery, List<AircraftSeatEntity>>
{
    public Task<List<AircraftSeatEntity>> Handle(GetAircraftSeatsQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftSeatEntity>().GetAllAsync(ct);
    }
}