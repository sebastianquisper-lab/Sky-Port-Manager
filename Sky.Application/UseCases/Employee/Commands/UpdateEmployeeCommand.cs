using MediatR;
using Sky.Domain.Ports;
using EmployeeEntity = Sky.Domain.Entities.Employee;

namespace Sky.Application.UseCases.Employee.Commands;

public sealed record UpdateEmployeeCommand(
    int EmployeeId,
    int AirportId,
    int DocumentTypeId,
    int EmployeeRoleId,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    DateOnly HireDate,
    bool IsActive) : IRequest<EmployeeEntity?>;

internal sealed class UpdateEmployeeHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateEmployeeCommand, EmployeeEntity?>
{
    public async Task<EmployeeEntity?> Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<EmployeeEntity>().GetByIdAsync(request.EmployeeId, ct);
        if (entity is null) return null;

        entity.AirportId = request.AirportId;
        entity.DocumentTypeId = request.DocumentTypeId;
        entity.EmployeeRoleId = request.EmployeeRoleId;
        entity.DocumentNumber = request.DocumentNumber;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.HireDate = request.HireDate;
        entity.IsActive = request.IsActive;

        uow.Repository<EmployeeEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
