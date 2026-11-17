namespace Sky.Application.UseCases.AircraftMaintenance.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;


public sealed record UpdateAircraftMaintenanceCommand(
    int MaintenanceId,
    int AircraftId,
    int MaintenanceTypeId,
    int AirportId,
    int? TechnicianEmployeeId,
    string Status,
    DateTime StartDateUtc,
    DateTime? EndDateUtc,
    string? Notes) : IRequest<AircraftMaintenanceEntity?>;

internal sealed class UpdateAircraftMaintenanceHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftMaintenanceCommand, AircraftMaintenanceEntity?>
{
    public async Task<AircraftMaintenanceEntity?> Handle(UpdateAircraftMaintenanceCommand request, CancellationToken ct)
    {
        var aircraftMaintenance = await uow.Repository<AircraftMaintenanceEntity>().GetByIdAsync(request.MaintenanceId, ct);
        if (aircraftMaintenance is null) return null;

        aircraftMaintenance.AircraftId = request.AircraftId;
        aircraftMaintenance.MaintenanceTypeId = request.MaintenanceTypeId;
        aircraftMaintenance.AirportId = request.AirportId;
        aircraftMaintenance.TechnicianEmployeeId = request.TechnicianEmployeeId;
        aircraftMaintenance.Status = request.Status;
        aircraftMaintenance.StartDateUtc = request.StartDateUtc;
        aircraftMaintenance.EndDateUtc = request.EndDateUtc;
        aircraftMaintenance.Notes = request.Notes;

        uow.Repository<AircraftMaintenanceEntity>().Update(aircraftMaintenance);
        await uow.SaveChangesAsync(ct);

        return aircraftMaintenance;
    }
}