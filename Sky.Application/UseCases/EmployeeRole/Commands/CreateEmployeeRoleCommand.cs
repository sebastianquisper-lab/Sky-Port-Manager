using MediatR;
using Sky.Domain.Ports;
using EmployeeRoleEntity = Sky.Domain.Entities.EmployeeRole;

namespace Sky.Application.UseCases.EmployeeRole.Commands;

public sealed record CreateEmployeeRoleCommand(
    string RoleName,
    string? Description) : IRequest<EmployeeRoleEntity>;

internal sealed class CreateEmployeeRoleHandler(IUnitOfWork uow)
    : IRequestHandler<CreateEmployeeRoleCommand, EmployeeRoleEntity>
{
    public async Task<EmployeeRoleEntity> Handle(CreateEmployeeRoleCommand request, CancellationToken ct)
    {
        var entity = new EmployeeRoleEntity
        {
            RoleName = request.RoleName,
            Description = request.Description
        };

        await uow.Repository<EmployeeRoleEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
