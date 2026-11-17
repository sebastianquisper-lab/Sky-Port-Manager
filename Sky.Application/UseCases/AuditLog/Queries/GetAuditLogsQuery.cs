using MediatR;
using Sky.Domain.Ports;
using AuditLogEntity = Sky.Domain.Entities.AuditLog;

namespace Sky.Application.UseCases.AuditLog.Queries;

public sealed record GetAuditLogsQuery : IRequest<IReadOnlyList<AuditLogEntity>>;

internal sealed class GetAuditLogsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAuditLogsQuery, IReadOnlyList<AuditLogEntity>>
{
    public Task<IReadOnlyList<AuditLogEntity>> Handle(GetAuditLogsQuery request, CancellationToken ct)
        => uow.Repository<AuditLogEntity>().GetAllAsync(ct);
}
