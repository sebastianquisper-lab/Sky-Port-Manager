using MediatR;
using Sky.Domain.Ports;
using EmployeeLicenseEntity = Sky.Domain.Entities.EmployeeLicense;

namespace Sky.Application.UseCases.EmployeeLicense.Commands;

public sealed record CreateEmployeeLicenseCommand(
    int EmployeeId,
    int IssuingCountryId,
    string LicenseType,
    string LicenseNumber,
    DateOnly IssueDate,
    DateOnly ExpiryDate) : IRequest<EmployeeLicenseEntity>;

internal sealed class CreateEmployeeLicenseHandler(IUnitOfWork uow)
    : IRequestHandler<CreateEmployeeLicenseCommand, EmployeeLicenseEntity>
{
    public async Task<EmployeeLicenseEntity> Handle(CreateEmployeeLicenseCommand request, CancellationToken ct)
    {
        var entity = new EmployeeLicenseEntity
        {
            EmployeeId = request.EmployeeId,
            IssuingCountryId = request.IssuingCountryId,
            LicenseType = request.LicenseType,
            LicenseNumber = request.LicenseNumber,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate
        };

        await uow.Repository<EmployeeLicenseEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
