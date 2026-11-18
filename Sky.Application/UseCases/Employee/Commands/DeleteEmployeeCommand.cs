using MediatR;
using Sky.Domain.Ports;
using EmployeeEntity = Sky.Domain.Entities.Employee;

namespace Sky.Application.UseCases.Employee.Commands;

public sealed record DeleteEmployeeCommand(
    int EmployeeId) : IRequest<bool>;

internal sealed class DeleteEmployeeHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteEmployeeCommand, bool>
{
    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<EmployeeEntity>().GetByIdAsync(request.EmployeeId, ct);
        if (entity is null) return false;

        uow.Repository<EmployeeEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
