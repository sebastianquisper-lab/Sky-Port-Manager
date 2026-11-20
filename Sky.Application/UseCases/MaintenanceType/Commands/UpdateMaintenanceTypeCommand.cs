using MediatR;
using Sky.Domain.Ports;
using MaintenanceTypeEntity = Sky.Domain.Entities.MaintenanceType;

namespace Sky.Application.UseCases.MaintenanceType.Commands;

public sealed record UpdateMaintenanceTypeCommand(
    int MaintenanceTypeId,
    string TypeName,
    string? Description) : IRequest<MaintenanceTypeEntity?>;

internal sealed class UpdateMaintenanceTypeHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateMaintenanceTypeCommand, MaintenanceTypeEntity?>
{
    public async Task<MaintenanceTypeEntity?> Handle(UpdateMaintenanceTypeCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<MaintenanceTypeEntity>().GetByIdAsync(request.MaintenanceTypeId, ct);
        if (entity is null) return null;

        entity.TypeName = request.TypeName;
        entity.Description = request.Description;

        uow.Repository<MaintenanceTypeEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
