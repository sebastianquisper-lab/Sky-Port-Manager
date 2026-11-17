namespace Sky.Application.UseCases.Baggage.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;


public sealed record UpdateBaggageCommand(
    int BaggageId,
    int TicketId,
    int BaggageTypeId,
    int BaggageStatusId,
    string TagNumber,
    decimal WeightKg,
    string? Description) : IRequest<BaggageEntity?>;

internal sealed class UpdateBaggageHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBaggageCommand, BaggageEntity?>
{
    public async Task<BaggageEntity?> Handle(UpdateBaggageCommand request, CancellationToken ct)
    {
        var baggage = await uow.Repository<BaggageEntity>().GetByIdAsync(request.BaggageId, ct);
        if (baggage is null) return null;

        baggage.TicketId = request.TicketId;
        baggage.BaggageTypeId = request.BaggageTypeId;
        baggage.BaggageStatusId = request.BaggageStatusId;
        baggage.TagNumber = request.TagNumber;
        baggage.WeightKg = request.WeightKg;
        baggage.Description = request.Description;

        uow.Repository<BaggageEntity>().Update(baggage);
        await uow.SaveChangesAsync(ct);

        return baggage;
    }
}