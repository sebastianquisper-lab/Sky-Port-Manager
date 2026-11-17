using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;

namespace Sky.Application.UseCases.Baggage.Commands;

public sealed record UpdateBaggageCommand(
    int BaggageId,
    int TicketId,
    int BaggageTypeId,
    int BaggageStatusId,
    decimal WeightKg,
    string? Description) : IRequest<BaggageEntity?>;

internal sealed class UpdateBaggageHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBaggageCommand, BaggageEntity?>
{
    public async Task<BaggageEntity?> Handle(UpdateBaggageCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<BaggageEntity>().GetByIdAsync(request.BaggageId, ct);
        if (entity is null) return null;

        entity.TicketId = request.TicketId;
        entity.BaggageTypeId = request.BaggageTypeId;
        entity.BaggageStatusId = request.BaggageStatusId;
        entity.WeightKg = request.WeightKg;
        entity.Description = request.Description;

        uow.Repository<BaggageEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
