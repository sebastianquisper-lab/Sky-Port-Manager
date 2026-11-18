using MediatR;
using Sky.Domain.Ports;
using EmployeeLicenseEntity = Sky.Domain.Entities.EmployeeLicense;

namespace Sky.Application.UseCases.EmployeeLicense.Commands;

public sealed record DeleteEmployeeLicenseCommand(
    int LicenseId) : IRequest<bool>;

internal sealed class DeleteEmployeeLicenseHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteEmployeeLicenseCommand, bool>
{
    public async Task<bool> Handle(DeleteEmployeeLicenseCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<EmployeeLicenseEntity>().GetByIdAsync(request.LicenseId, ct);
        if (entity is null) return false;

        uow.Repository<EmployeeLicenseEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
