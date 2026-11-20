using MediatR;
using Sky.Domain.Ports;
using MaintenanceTypeEntity = Sky.Domain.Entities.MaintenanceType;

namespace Sky.Application.UseCases.MaintenanceType.Commands;

public sealed record DeleteMaintenanceTypeCommand(
    int MaintenanceTypeId) : IRequest<bool>;

internal sealed class DeleteMaintenanceTypeHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteMaintenanceTypeCommand, bool>
{
    public async Task<bool> Handle(DeleteMaintenanceTypeCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<MaintenanceTypeEntity>().GetByIdAsync(request.MaintenanceTypeId, ct);
        if (entity is null) return false;

        uow.Repository<MaintenanceTypeEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
