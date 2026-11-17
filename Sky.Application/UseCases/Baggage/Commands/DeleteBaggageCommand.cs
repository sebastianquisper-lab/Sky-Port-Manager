using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;

namespace Sky.Application.UseCases.Baggage.Commands;

public sealed record DeleteBaggageCommand(
    int BaggageId) : IRequest<bool>;

internal sealed class DeleteBaggageHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBaggageCommand, bool>
{
    public async Task<bool> Handle(DeleteBaggageCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<BaggageEntity>().GetByIdAsync(request.BaggageId, ct);
        if (entity is null) return false;

        uow.Repository<BaggageEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
