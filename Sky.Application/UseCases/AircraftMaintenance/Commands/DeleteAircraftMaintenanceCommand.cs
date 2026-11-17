namespace Sky.Application.UseCases.AircraftMaintenance.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;


public sealed record DeleteAircraftMaintenanceCommand(int MaintenanceId) : IRequest<bool>;

internal sealed class DeleteAircraftMaintenanceHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftMaintenanceCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftMaintenanceCommand request, CancellationToken ct)
    {
        var aircraftMaintenance = await uow.Repository<AircraftMaintenanceEntity>().GetByIdAsync(request.MaintenanceId, ct);
        if (aircraftMaintenance is null) return false;

        uow.Repository<AircraftMaintenanceEntity>().Delete(aircraftMaintenance);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}