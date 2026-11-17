using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;

namespace Sky.Application.UseCases.AircraftSeat.Queries;

public sealed record GetAircraftSeatByIdQuery(
    int AircraftSeatId) : IRequest<AircraftSeatEntity?>;

internal sealed class GetAircraftSeatByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftSeatByIdQuery, AircraftSeatEntity?>
{
    public Task<AircraftSeatEntity?> Handle(GetAircraftSeatByIdQuery request, CancellationToken ct)
        => uow.Repository<AircraftSeatEntity>().GetByIdAsync(request.AircraftSeatId, ct);
}
