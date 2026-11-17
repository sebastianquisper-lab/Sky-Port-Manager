using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;

namespace Sky.Application.UseCases.AircraftSeat.Commands;

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
        var entity = await uow.Repository<AircraftSeatEntity>().GetByIdAsync(request.AircraftSeatId, ct);
        if (entity is null) return null;

        entity.AircraftId = request.AircraftId;
        entity.CabinClassId = request.CabinClassId;
        entity.SeatNumber = request.SeatNumber;
        entity.IsWindow = request.IsWindow;
        entity.IsAisle = request.IsAisle;
        entity.IsEmergencyExit = request.IsEmergencyExit;

        uow.Repository<AircraftSeatEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
