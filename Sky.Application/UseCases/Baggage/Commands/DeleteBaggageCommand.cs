namespace Sky.Application.UseCases.Baggage.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;


public sealed record DeleteBaggageCommand(int BaggageId) : IRequest<bool>;

internal sealed class DeleteBaggageHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBaggageCommand, bool>
{
    public async Task<bool> Handle(DeleteBaggageCommand request, CancellationToken ct)
    {
        var baggage = await uow.Repository<BaggageEntity>().GetByIdAsync(request.BaggageId, ct);
        if (baggage is null) return false;

        uow.Repository<BaggageEntity>().Delete(baggage);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}