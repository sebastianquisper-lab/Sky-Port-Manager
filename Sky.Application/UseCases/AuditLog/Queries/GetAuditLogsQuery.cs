namespace Sky.Application.UseCases.AuditLog.Queries;

using MediatR;
using Sky.Domain.Ports;
using AuditLogEntity = Sky.Domain.Entities.AuditLog;


public sealed record GetAuditLogsQuery : IRequest<List<AuditLogEntity>>;

internal sealed class GetAuditLogsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAuditLogsQuery, List<AuditLogEntity>>
{
    public Task<List<AuditLogEntity>> Handle(GetAuditLogsQuery request, CancellationToken ct)
    {
        return uow.Repository<AuditLogEntity>().GetAllAsync(ct);
    }
}