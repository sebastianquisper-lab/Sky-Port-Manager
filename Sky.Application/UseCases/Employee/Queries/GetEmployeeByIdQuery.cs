using MediatR;
using Sky.Domain.Ports;
using EmployeeEntity = Sky.Domain.Entities.Employee;

namespace Sky.Application.UseCases.Employee.Queries;

public sealed record GetEmployeeByIdQuery(
    int EmployeeId) : IRequest<EmployeeEntity?>;

internal sealed class GetEmployeeByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeEntity?>
{
    public Task<EmployeeEntity?> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
        => uow.Repository<EmployeeEntity>().GetByIdAsync(request.EmployeeId, ct);
}
