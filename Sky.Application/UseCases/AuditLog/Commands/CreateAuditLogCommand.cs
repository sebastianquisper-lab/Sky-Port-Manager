using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AuditLogEntity = Sky.Domain.Entities.AuditLog;

namespace Sky.Application.UseCases.AuditLog.Commands;

public sealed record CreateAuditLogCommand(
    string TableName,
    string RecordId,
    string Action,
    string? OldValues,
    string? NewValues) : IRequest<AuditLogEntity>;

internal sealed class CreateAuditLogHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateAuditLogCommand, AuditLogEntity>
{
    public async Task<AuditLogEntity> Handle(CreateAuditLogCommand request, CancellationToken ct)
    {
        var entity = new AuditLogEntity
        {
            UserId = currentUser.UserId,
            TableName = request.TableName,
            RecordId = request.RecordId,
            Action = request.Action,
            OldValues = request.OldValues,
            NewValues = request.NewValues,
            IpAddress = currentUser.IpAddress,
            CreatedAtUtc = clock.UtcNow
        };

        await uow.Repository<AuditLogEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
