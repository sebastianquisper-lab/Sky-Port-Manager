using MediatR;
using Sky.Domain.Ports;
using MaintenanceTypeEntity = Sky.Domain.Entities.MaintenanceType;

namespace Sky.Application.UseCases.MaintenanceType.Queries;

public sealed record GetMaintenanceTypeByIdQuery(
    int MaintenanceTypeId) : IRequest<MaintenanceTypeEntity?>;

internal sealed class GetMaintenanceTypeByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetMaintenanceTypeByIdQuery, MaintenanceTypeEntity?>
{
    public Task<MaintenanceTypeEntity?> Handle(GetMaintenanceTypeByIdQuery request, CancellationToken ct)
        => uow.Repository<MaintenanceTypeEntity>().GetByIdAsync(request.MaintenanceTypeId, ct);
}
