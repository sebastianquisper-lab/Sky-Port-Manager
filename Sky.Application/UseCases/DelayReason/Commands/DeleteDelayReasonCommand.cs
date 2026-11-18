using MediatR;
using Sky.Domain.Ports;
using DelayReasonEntity = Sky.Domain.Entities.DelayReason;

namespace Sky.Application.UseCases.DelayReason.Commands;

public sealed record DeleteDelayReasonCommand(
    int DelayReasonId) : IRequest<bool>;

internal sealed class DeleteDelayReasonHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteDelayReasonCommand, bool>
{
    public async Task<bool> Handle(DeleteDelayReasonCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<DelayReasonEntity>().GetByIdAsync(request.DelayReasonId, ct);
        if (entity is null) return false;

        uow.Repository<DelayReasonEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
