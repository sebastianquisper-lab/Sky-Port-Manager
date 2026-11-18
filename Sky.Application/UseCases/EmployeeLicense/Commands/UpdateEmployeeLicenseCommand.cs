using MediatR;
using Sky.Domain.Ports;
using EmployeeLicenseEntity = Sky.Domain.Entities.EmployeeLicense;

namespace Sky.Application.UseCases.EmployeeLicense.Commands;

public sealed record UpdateEmployeeLicenseCommand(
    int LicenseId,
    int EmployeeId,
    int IssuingCountryId,
    string LicenseType,
    string LicenseNumber,
    DateOnly IssueDate,
    DateOnly ExpiryDate) : IRequest<EmployeeLicenseEntity?>;

internal sealed class UpdateEmployeeLicenseHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateEmployeeLicenseCommand, EmployeeLicenseEntity?>
{
    public async Task<EmployeeLicenseEntity?> Handle(UpdateEmployeeLicenseCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<EmployeeLicenseEntity>().GetByIdAsync(request.LicenseId, ct);
        if (entity is null) return null;

        entity.EmployeeId = request.EmployeeId;
        entity.IssuingCountryId = request.IssuingCountryId;
        entity.LicenseType = request.LicenseType;
        entity.LicenseNumber = request.LicenseNumber;
        entity.IssueDate = request.IssueDate;
        entity.ExpiryDate = request.ExpiryDate;

        uow.Repository<EmployeeLicenseEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
