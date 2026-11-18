using MediatR;
using Sky.Domain.Ports;
using EmployeeRoleEntity = Sky.Domain.Entities.EmployeeRole;

namespace Sky.Application.UseCases.EmployeeRole.Queries;

public sealed record GetEmployeeRoleByIdQuery(
    int EmployeeRoleId) : IRequest<EmployeeRoleEntity?>;

internal sealed class GetEmployeeRoleByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetEmployeeRoleByIdQuery, EmployeeRoleEntity?>
{
    public Task<EmployeeRoleEntity?> Handle(GetEmployeeRoleByIdQuery request, CancellationToken ct)
        => uow.Repository<EmployeeRoleEntity>().GetByIdAsync(request.EmployeeRoleId, ct);
}
