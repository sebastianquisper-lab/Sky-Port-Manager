namespace Sky.Application.UseCases.AircraftMaintenance.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;


public sealed record GetAircraftMaintenanceByIdQuery(int MaintenanceId) : IRequest<AircraftMaintenanceEntity?>;

internal sealed class GetAircraftMaintenanceByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftMaintenanceByIdQuery, AircraftMaintenanceEntity?>
{
    public Task<AircraftMaintenanceEntity?> Handle(GetAircraftMaintenanceByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftMaintenanceEntity>().GetByIdAsync(request.MaintenanceId, ct);
    }
}