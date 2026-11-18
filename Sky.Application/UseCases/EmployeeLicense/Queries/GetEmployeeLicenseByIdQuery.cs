using MediatR;
using Sky.Domain.Ports;
using EmployeeLicenseEntity = Sky.Domain.Entities.EmployeeLicense;

namespace Sky.Application.UseCases.EmployeeLicense.Queries;

public sealed record GetEmployeeLicenseByIdQuery(
    int LicenseId) : IRequest<EmployeeLicenseEntity?>;

internal sealed class GetEmployeeLicenseByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetEmployeeLicenseByIdQuery, EmployeeLicenseEntity?>
{
    public Task<EmployeeLicenseEntity?> Handle(GetEmployeeLicenseByIdQuery request, CancellationToken ct)
        => uow.Repository<EmployeeLicenseEntity>().GetByIdAsync(request.LicenseId, ct);
}
