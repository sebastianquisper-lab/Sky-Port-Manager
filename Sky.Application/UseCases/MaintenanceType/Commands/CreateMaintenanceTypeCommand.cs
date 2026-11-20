using MediatR;
using Sky.Domain.Ports;
using MaintenanceTypeEntity = Sky.Domain.Entities.MaintenanceType;

namespace Sky.Application.UseCases.MaintenanceType.Commands;

public sealed record CreateMaintenanceTypeCommand(
    string TypeName,
    string? Description) : IRequest<MaintenanceTypeEntity>;

internal sealed class CreateMaintenanceTypeHandler(IUnitOfWork uow)
    : IRequestHandler<CreateMaintenanceTypeCommand, MaintenanceTypeEntity>
{
    public async Task<MaintenanceTypeEntity> Handle(CreateMaintenanceTypeCommand request, CancellationToken ct)
    {
        var entity = new MaintenanceTypeEntity
        {
            TypeName = request.TypeName,
            Description = request.Description
        };

        await uow.Repository<MaintenanceTypeEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
