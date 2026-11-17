using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;

namespace Sky.Application.UseCases.AircraftMaintenance.Commands;

public sealed record CreateAircraftMaintenanceCommand(
    int AircraftId,
    int MaintenanceTypeId,
    int AirportId,
    int? TechnicianEmployeeId,
    string Status,
    DateTime StartDateUtc,
    DateTime? EndDateUtc,
    string? Notes) : IRequest<AircraftMaintenanceEntity>;

internal sealed class CreateAircraftMaintenanceHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftMaintenanceCommand, AircraftMaintenanceEntity>
{
    public async Task<AircraftMaintenanceEntity> Handle(CreateAircraftMaintenanceCommand request, CancellationToken ct)
    {
        var entity = new AircraftMaintenanceEntity
        {
            AircraftId = request.AircraftId,
            MaintenanceTypeId = request.MaintenanceTypeId,
            AirportId = request.AirportId,
            TechnicianEmployeeId = request.TechnicianEmployeeId,
            Status = request.Status,
            StartDateUtc = request.StartDateUtc,
            EndDateUtc = request.EndDateUtc,
            Notes = request.Notes
        };

        await uow.Repository<AircraftMaintenanceEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
