using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;

namespace Sky.Application.UseCases.AircraftMaintenance.Queries;

public sealed record GetAircraftMaintenanceByIdQuery(
    int MaintenanceId) : IRequest<AircraftMaintenanceEntity?>;

internal sealed class GetAircraftMaintenanceByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftMaintenanceByIdQuery, AircraftMaintenanceEntity?>
{
    public Task<AircraftMaintenanceEntity?> Handle(GetAircraftMaintenanceByIdQuery request, CancellationToken ct)
        => uow.Repository<AircraftMaintenanceEntity>().GetByIdAsync(request.MaintenanceId, ct);
}
