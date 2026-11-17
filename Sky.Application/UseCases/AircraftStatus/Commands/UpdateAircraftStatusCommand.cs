namespace Sky.Application.UseCases.AircraftStatus.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;


public sealed record UpdateAircraftStatusCommand(
    int AircraftStatusId,
    string StatusName,
    string? Description) : IRequest<AircraftStatusEntity?>;

internal sealed class UpdateAircraftStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftStatusCommand, AircraftStatusEntity?>
{
    public async Task<AircraftStatusEntity?> Handle(UpdateAircraftStatusCommand request, CancellationToken ct)
    {
        var aircraftStatus = await uow.Repository<AircraftStatusEntity>().GetByIdAsync(request.AircraftStatusId, ct);
        if (aircraftStatus is null) return null;

        aircraftStatus.StatusName = request.StatusName;
        aircraftStatus.Description = request.Description;

        uow.Repository<AircraftStatusEntity>().Update(aircraftStatus);
        await uow.SaveChangesAsync(ct);

        return aircraftStatus;
    }
}