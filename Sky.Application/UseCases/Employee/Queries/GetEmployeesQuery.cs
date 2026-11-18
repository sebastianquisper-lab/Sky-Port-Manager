using MediatR;
using Sky.Domain.Ports;
using EmployeeEntity = Sky.Domain.Entities.Employee;

namespace Sky.Application.UseCases.Employee.Queries;

public sealed record GetEmployeesQuery : IRequest<IReadOnlyList<EmployeeEntity>>;

internal sealed class GetEmployeesHandler(IUnitOfWork uow)
    : IRequestHandler<GetEmployeesQuery, IReadOnlyList<EmployeeEntity>>
{
    public Task<IReadOnlyList<EmployeeEntity>> Handle(GetEmployeesQuery request, CancellationToken ct)
        => uow.Repository<EmployeeEntity>().GetAllAsync(ct);
}
