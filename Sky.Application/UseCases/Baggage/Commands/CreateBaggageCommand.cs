namespace Sky.Application.UseCases.Baggage.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;


public sealed record CreateBaggageCommand(
    int TicketId,
    int BaggageTypeId,
    int BaggageStatusId,
    string TagNumber,
    decimal WeightKg,
    string? Description) : IRequest<BaggageEntity>;

internal sealed class CreateBaggageHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBaggageCommand, BaggageEntity>
{
    public async Task<BaggageEntity> Handle(CreateBaggageCommand request, CancellationToken ct)
    {
        var baggage = new BaggageEntity
        {
            TicketId = request.TicketId,
            BaggageTypeId = request.BaggageTypeId,
            BaggageStatusId = request.BaggageStatusId,
            TagNumber = request.TagNumber,
            WeightKg = request.WeightKg,
            Description = request.Description,
            CreatedAtUtc = DateTime.UtcNow
        };

        await uow.Repository<BaggageEntity>().AddAsync(baggage, ct);
        await uow.SaveChangesAsync(ct);

        return baggage;
    }
}