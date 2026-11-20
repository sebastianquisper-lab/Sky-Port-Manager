using MediatR;
using Sky.Domain.Ports;
using VLicensesExpiringEntity = Sky.Domain.Entities.VLicensesExpiring;

namespace Sky.Application.UseCases.VLicensesExpiring.Queries;

public sealed record GetLicensesExpiringQuery : IRequest<IReadOnlyList<VLicensesExpiringEntity>>;

internal sealed class GetLicensesExpiringHandler(IUnitOfWork uow)
    : IRequestHandler<GetLicensesExpiringQuery, IReadOnlyList<VLicensesExpiringEntity>>
{
    public Task<IReadOnlyList<VLicensesExpiringEntity>> Handle(GetLicensesExpiringQuery request, CancellationToken ct)
        => uow.Repository<VLicensesExpiringEntity>().GetAllAsync(ct);
}
