namespace Sky.Application.UseCases.AircraftSeat.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;


public sealed record DeleteAircraftSeatCommand(int AircraftSeatId) : IRequest<bool>;

internal sealed class DeleteAircraftSeatHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftSeatCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftSeatCommand request, CancellationToken ct)
    {
        var aircraftSeat = await uow.Repository<AircraftSeatEntity>().GetByIdAsync(request.AircraftSeatId, ct);
        if (aircraftSeat is null) return false;

        uow.Repository<AircraftSeatEntity>().Delete(aircraftSeat);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}