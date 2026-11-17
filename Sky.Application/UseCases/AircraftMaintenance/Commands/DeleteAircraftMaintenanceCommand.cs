using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;

namespace Sky.Application.UseCases.AircraftMaintenance.Commands;

public sealed record DeleteAircraftMaintenanceCommand(
    int MaintenanceId) : IRequest<bool>;

internal sealed class DeleteAircraftMaintenanceHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftMaintenanceCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftMaintenanceCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftMaintenanceEntity>().GetByIdAsync(request.MaintenanceId, ct);
        if (entity is null) return false;

        uow.Repository<AircraftMaintenanceEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
