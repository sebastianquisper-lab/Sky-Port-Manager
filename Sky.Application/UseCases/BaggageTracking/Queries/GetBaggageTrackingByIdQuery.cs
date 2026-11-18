namespace Sky.Application.UseCases.BaggageTracking.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageTrackingEntity = Sky.Domain.Entities.BaggageTracking;


public sealed record GetBaggageTrackingByIdQuery(int TrackingId) : IRequest<BaggageTrackingEntity?>;

internal sealed class GetBaggageTrackingByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageTrackingByIdQuery, BaggageTrackingEntity?>
{
    public Task<BaggageTrackingEntity?> Handle(GetBaggageTrackingByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageTrackingEntity>().GetByIdAsync(request.TrackingId, ct);
    }
}