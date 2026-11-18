namespace Sky.Application.UseCases.BaggageTracking.Commands;

using MediatR;
using Sky.Domain.Ports;
using BaggageTrackingEntity = Sky.Domain.Entities.BaggageTracking;


public sealed record DeleteBaggageTrackingCommand(int TrackingId) : IRequest<bool>;

internal sealed class DeleteBaggageTrackingHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBaggageTrackingCommand, bool>
{
    public async Task<bool> Handle(DeleteBaggageTrackingCommand request, CancellationToken ct)
    {
        var baggageTracking = await uow.Repository<BaggageTrackingEntity>().GetByIdAsync(request.TrackingId, ct);
        if (baggageTracking is null) return false;

        uow.Repository<BaggageTrackingEntity>().Delete(baggageTracking);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}