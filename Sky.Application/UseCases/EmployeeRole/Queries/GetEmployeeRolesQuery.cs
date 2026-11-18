using MediatR;
using Sky.Domain.Ports;
using EmployeeRoleEntity = Sky.Domain.Entities.EmployeeRole;

namespace Sky.Application.UseCases.EmployeeRole.Queries;

public sealed record GetEmployeeRolesQuery : IRequest<IReadOnlyList<EmployeeRoleEntity>>;

internal sealed class GetEmployeeRolesHandler(IUnitOfWork uow)
    : IRequestHandler<GetEmployeeRolesQuery, IReadOnlyList<EmployeeRoleEntity>>
{
    public Task<IReadOnlyList<EmployeeRoleEntity>> Handle(GetEmployeeRolesQuery request, CancellationToken ct)
        => uow.Repository<EmployeeRoleEntity>().GetAllAsync(ct);
}
