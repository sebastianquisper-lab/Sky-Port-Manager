namespace Sky.Application.UseCases.AuditLog.Commands;

using MediatR;
using Sky.Domain.Ports;
using AuditLogEntity = Sky.Domain.Entities.AuditLog;


public sealed record CreateAuditLogCommand(
    int? UserId,
    string TableName,
    string RecordId,
    string Action,
    string? OldValues,
    string? NewValues,
    string? IpAddress) : IRequest<AuditLogEntity>;

internal sealed class CreateAuditLogHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAuditLogCommand, AuditLogEntity>
{
    public async Task<AuditLogEntity> Handle(CreateAuditLogCommand request, CancellationToken ct)
    {
        var auditLog = new AuditLogEntity
        {
            UserId = request.UserId,
            TableName = request.TableName,
            RecordId = request.RecordId,
            Action = request.Action,
            OldValues = request.OldValues,
            NewValues = request.NewValues,
            IpAddress = request.IpAddress,
            CreatedAtUtc = DateTime.UtcNow
        };

        await uow.Repository<AuditLogEntity>().AddAsync(auditLog, ct);
        await uow.SaveChangesAsync(ct);

        return auditLog;
    }
}