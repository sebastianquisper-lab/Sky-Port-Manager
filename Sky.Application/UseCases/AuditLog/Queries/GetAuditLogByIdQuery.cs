namespace Sky.Application.UseCases.AuditLog.Queries;

using MediatR;
using Sky.Domain.Ports;
using AuditLogEntity = Sky.Domain.Entities.AuditLog;


public sealed record GetAuditLogByIdQuery(long AuditId) : IRequest<AuditLogEntity?>;

internal sealed class GetAuditLogByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAuditLogByIdQuery, AuditLogEntity?>
{
    public Task<AuditLogEntity?> Handle(GetAuditLogByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<AuditLogEntity>().GetByIdAsync(request.AuditId, ct);
    }
}