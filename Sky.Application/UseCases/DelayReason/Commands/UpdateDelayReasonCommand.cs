using MediatR;
using Sky.Domain.Ports;
using DelayReasonEntity = Sky.Domain.Entities.DelayReason;

namespace Sky.Application.UseCases.DelayReason.Commands;

public sealed record UpdateDelayReasonCommand(
    int DelayReasonId,
    string ReasonCode,
    string Description) : IRequest<DelayReasonEntity?>;

internal sealed class UpdateDelayReasonHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateDelayReasonCommand, DelayReasonEntity?>
{
    public async Task<DelayReasonEntity?> Handle(UpdateDelayReasonCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<DelayReasonEntity>().GetByIdAsync(request.DelayReasonId, ct);
        if (entity is null) return null;

        entity.ReasonCode = request.ReasonCode;
        entity.Description = request.Description;

        uow.Repository<DelayReasonEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
