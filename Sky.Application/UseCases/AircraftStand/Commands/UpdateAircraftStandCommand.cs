namespace Sky.Application.UseCases.AircraftStand.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;


public sealed record UpdateAircraftStandCommand(
    int StandId,
    int AirportId,
    string StandCode,
    bool IsRemote,
    bool IsActive) : IRequest<AircraftStandEntity?>;

internal sealed class UpdateAircraftStandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftStandCommand, AircraftStandEntity?>
{
    public async Task<AircraftStandEntity?> Handle(UpdateAircraftStandCommand request, CancellationToken ct)
    {
        var aircraftStand = await uow.Repository<AircraftStandEntity>().GetByIdAsync(request.StandId, ct);
        if (aircraftStand is null) return null;

        aircraftStand.AirportId = request.AirportId;
        aircraftStand.StandCode = request.StandCode;
        aircraftStand.IsRemote = request.IsRemote;
        aircraftStand.IsActive = request.IsActive;

        uow.Repository<AircraftStandEntity>().Update(aircraftStand);
        await uow.SaveChangesAsync(ct);

        return aircraftStand;
    }
}