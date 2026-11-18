using MediatR;
using Sky.Domain.Ports;
using EmployeeLicenseEntity = Sky.Domain.Entities.EmployeeLicense;

namespace Sky.Application.UseCases.EmployeeLicense.Queries;

public sealed record GetEmployeeLicensesQuery : IRequest<IReadOnlyList<EmployeeLicenseEntity>>;

internal sealed class GetEmployeeLicensesHandler(IUnitOfWork uow)
    : IRequestHandler<GetEmployeeLicensesQuery, IReadOnlyList<EmployeeLicenseEntity>>
{
    public Task<IReadOnlyList<EmployeeLicenseEntity>> Handle(GetEmployeeLicensesQuery request, CancellationToken ct)
        => uow.Repository<EmployeeLicenseEntity>().GetAllAsync(ct);
}
