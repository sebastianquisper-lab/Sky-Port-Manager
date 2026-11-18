using MediatR;
using Sky.Domain.Ports;
using EmployeeRoleEntity = Sky.Domain.Entities.EmployeeRole;

namespace Sky.Application.UseCases.EmployeeRole.Commands;

public sealed record DeleteEmployeeRoleCommand(
    int EmployeeRoleId) : IRequest<bool>;

internal sealed class DeleteEmployeeRoleHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteEmployeeRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteEmployeeRoleCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<EmployeeRoleEntity>().GetByIdAsync(request.EmployeeRoleId, ct);
        if (entity is null) return false;

        uow.Repository<EmployeeRoleEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
