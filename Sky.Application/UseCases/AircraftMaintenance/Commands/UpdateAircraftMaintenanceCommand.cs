using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;

namespace Sky.Application.UseCases.AircraftMaintenance.Commands;

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
        var entity = await uow.Repository<AircraftMaintenanceEntity>().GetByIdAsync(request.MaintenanceId, ct);
        if (entity is null) return null;

        entity.AircraftId = request.AircraftId;
        entity.MaintenanceTypeId = request.MaintenanceTypeId;
        entity.AirportId = request.AirportId;
        entity.TechnicianEmployeeId = request.TechnicianEmployeeId;
        entity.Status = request.Status;
        entity.StartDateUtc = request.StartDateUtc;
        entity.EndDateUtc = request.EndDateUtc;
        entity.Notes = request.Notes;

        uow.Repository<AircraftMaintenanceEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
