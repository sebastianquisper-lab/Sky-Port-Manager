namespace Sky.Application.UseCases.AircraftSeat.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;


public sealed record UpdateAircraftSeatCommand(
    int AircraftSeatId,
    int AircraftId,
    int CabinClassId,
    string SeatNumber,
    bool IsWindow,
    bool IsAisle,
    bool IsEmergencyExit) : IRequest<AircraftSeatEntity?>;

internal sealed class UpdateAircraftSeatHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftSeatCommand, AircraftSeatEntity?>
{
    public async Task<AircraftSeatEntity?> Handle(UpdateAircraftSeatCommand request, CancellationToken ct)
    {
        var aircraftSeat = await uow.Repository<AircraftSeatEntity>().GetByIdAsync(request.AircraftSeatId, ct);
        if (aircraftSeat is null) return null;

        aircraftSeat.AircraftId = request.AircraftId;
        aircraftSeat.CabinClassId = request.CabinClassId;
        aircraftSeat.SeatNumber = request.SeatNumber;
        aircraftSeat.IsWindow = request.IsWindow;
        aircraftSeat.IsAisle = request.IsAisle;
        aircraftSeat.IsEmergencyExit = request.IsEmergencyExit;

        uow.Repository<AircraftSeatEntity>().Update(aircraftSeat);
        await uow.SaveChangesAsync(ct);

        return aircraftSeat;
    }
}