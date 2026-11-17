using MediatR;
using Sky.Domain.Ports;
using AircraftSeatEntity = Sky.Domain.Entities.AircraftSeat;

namespace Sky.Application.UseCases.AircraftSeat.Commands;

public sealed record CreateAircraftSeatCommand(
    int AircraftId,
    int CabinClassId,
    string SeatNumber,
    bool IsWindow,
    bool IsAisle,
    bool IsEmergencyExit) : IRequest<AircraftSeatEntity>;

internal sealed class CreateAircraftSeatHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftSeatCommand, AircraftSeatEntity>
{
    public async Task<AircraftSeatEntity> Handle(CreateAircraftSeatCommand request, CancellationToken ct)
    {
        var entity = new AircraftSeatEntity
        {
            AircraftId = request.AircraftId,
            CabinClassId = request.CabinClassId,
            SeatNumber = request.SeatNumber,
            IsWindow = request.IsWindow,
            IsAisle = request.IsAisle,
            IsEmergencyExit = request.IsEmergencyExit
        };

        await uow.Repository<AircraftSeatEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
