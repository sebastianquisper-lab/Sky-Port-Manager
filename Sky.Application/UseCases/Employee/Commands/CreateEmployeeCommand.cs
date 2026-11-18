using MediatR;
using Sky.Domain.Ports;
using EmployeeEntity = Sky.Domain.Entities.Employee;

namespace Sky.Application.UseCases.Employee.Commands;

public sealed record CreateEmployeeCommand(
    int AirportId,
    int DocumentTypeId,
    int EmployeeRoleId,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    DateOnly HireDate) : IRequest<EmployeeEntity>;

internal sealed class CreateEmployeeHandler(IUnitOfWork uow)
    : IRequestHandler<CreateEmployeeCommand, EmployeeEntity>
{
    public async Task<EmployeeEntity> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        var entity = new EmployeeEntity
        {
            AirportId = request.AirportId,
            DocumentTypeId = request.DocumentTypeId,
            EmployeeRoleId = request.EmployeeRoleId,
            DocumentNumber = request.DocumentNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            HireDate = request.HireDate,
            IsActive = true
        };

        await uow.Repository<EmployeeEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
