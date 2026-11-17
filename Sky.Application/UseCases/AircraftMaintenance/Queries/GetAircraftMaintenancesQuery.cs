using MediatR;
using Sky.Domain.Ports;
using AircraftMaintenanceEntity = Sky.Domain.Entities.AircraftMaintenance;

namespace Sky.Application.UseCases.AircraftMaintenance.Queries;

public sealed record GetAircraftMaintenancesQuery : IRequest<IReadOnlyList<AircraftMaintenanceEntity>>;

internal sealed class GetAircraftMaintenancesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftMaintenancesQuery, IReadOnlyList<AircraftMaintenanceEntity>>
{
    public Task<IReadOnlyList<AircraftMaintenanceEntity>> Handle(GetAircraftMaintenancesQuery request, CancellationToken ct)
        => uow.Repository<AircraftMaintenanceEntity>().GetAllAsync(ct);
}
