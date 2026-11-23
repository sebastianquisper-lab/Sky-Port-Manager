namespace Sky.Application.UseCases.BaggageTracking.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageTrackingEntity = Sky.Domain.Entities.BaggageTracking;


public sealed record GetBaggageTrackingsQuery : IRequest<IReadOnlyList<BaggageTrackingEntity>>;

internal sealed class GetBaggageTrackingsHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageTrackingsQuery, IReadOnlyList<BaggageTrackingEntity>>
{
    public Task<IReadOnlyList<BaggageTrackingEntity>> Handle(GetBaggageTrackingsQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageTrackingEntity>().GetAllAsync(ct);
    }
}