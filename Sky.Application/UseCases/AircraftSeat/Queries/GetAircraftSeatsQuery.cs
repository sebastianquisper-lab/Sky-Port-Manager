using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;

namespace Sky.Application.UseCases.AircraftSeat.Queries;

public sealed record GetAircraftSeatsQuery : IRequest<IReadOnlyList<AircraftSeatEntity>>;

internal sealed class GetAircraftSeatsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftSeatsQuery, IReadOnlyList<AircraftSeatEntity>>
{
    public Task<IReadOnlyList<AircraftSeatEntity>> Handle(GetAircraftSeatsQuery request, CancellationToken ct)
        => uow.Repository<AircraftSeatEntity>().GetAllAsync(ct);
}
