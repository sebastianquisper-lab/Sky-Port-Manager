using MediatR;
using Sky.Domain.Ports;
using DelayReasonEntity = Sky.Domain.Entities.DelayReason;

namespace Sky.Application.UseCases.DelayReason.Commands;

public sealed record CreateDelayReasonCommand(
    string ReasonCode,
    string Description) : IRequest<DelayReasonEntity>;

internal sealed class CreateDelayReasonHandler(IUnitOfWork uow)
    : IRequestHandler<CreateDelayReasonCommand, DelayReasonEntity>
{
    public async Task<DelayReasonEntity> Handle(CreateDelayReasonCommand request, CancellationToken ct)
    {
        var entity = new DelayReasonEntity
        {
            ReasonCode = request.ReasonCode,
            Description = request.Description
        };

        await uow.Repository<DelayReasonEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
