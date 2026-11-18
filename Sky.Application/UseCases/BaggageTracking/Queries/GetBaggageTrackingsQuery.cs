namespace Sky.Application.UseCases.BaggageTracking.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageTrackingEntity = Sky.Domain.Entities.BaggageTracking;


public sealed record GetBaggageTrackingsQuery : IRequest<List<BaggageTrackingEntity>>;

internal sealed class GetBaggageTrackingsHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageTrackingsQuery, List<BaggageTrackingEntity>>
{
    public Task<List<BaggageTrackingEntity>> Handle(GetBaggageTrackingsQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageTrackingEntity>().GetAllAsync(ct);
    }
}