namespace Sky.Application.UseCases.AircraftMaintenance.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;


public sealed record GetAircraftMaintenancesQuery : IRequest<List<AircraftMaintenanceEntity>>;

internal sealed class GetAircraftMaintenancesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftMaintenancesQuery, List<AircraftMaintenanceEntity>>
{
    public Task<List<AircraftMaintenanceEntity>> Handle(GetAircraftMaintenancesQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftMaintenanceEntity>().GetAllAsync(ct);
    }
}