using MediatR;
using Sky.Domain.Ports;
using MaintenanceTypeEntity = Sky.Domain.Entities.MaintenanceType;

namespace Sky.Application.UseCases.MaintenanceType.Queries;

public sealed record GetMaintenanceTypesQuery : IRequest<IReadOnlyList<MaintenanceTypeEntity>>;

internal sealed class GetMaintenanceTypesHandler(IUnitOfWork uow)
    : IRequestHandler<GetMaintenanceTypesQuery, IReadOnlyList<MaintenanceTypeEntity>>
{
    public Task<IReadOnlyList<MaintenanceTypeEntity>> Handle(GetMaintenanceTypesQuery request, CancellationToken ct)
        => uow.Repository<MaintenanceTypeEntity>().GetAllAsync(ct);
}
