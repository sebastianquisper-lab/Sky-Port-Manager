using MediatR;
using Sky.Domain.Ports;
using EmployeeRoleEntity = Sky.Domain.Entities.EmployeeRole;

namespace Sky.Application.UseCases.EmployeeRole.Commands;

public sealed record UpdateEmployeeRoleCommand(
    int EmployeeRoleId,
    string RoleName,
    string? Description) : IRequest<EmployeeRoleEntity?>;

internal sealed class UpdateEmployeeRoleHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateEmployeeRoleCommand, EmployeeRoleEntity?>
{
    public async Task<EmployeeRoleEntity?> Handle(UpdateEmployeeRoleCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<EmployeeRoleEntity>().GetByIdAsync(request.EmployeeRoleId, ct);
        if (entity is null) return null;

        entity.RoleName = request.RoleName;
        entity.Description = request.Description;

        uow.Repository<EmployeeRoleEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
