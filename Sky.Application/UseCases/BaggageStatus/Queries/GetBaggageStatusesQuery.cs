namespace Sky.Application.UseCases.BaggageStatus.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageStatusEntity = Sky.Domain.Entities.BaggageStatus;


public sealed record GetBaggageStatusesQuery : IRequest<IReadOnlyList<BaggageStatusEntity>>;

internal sealed class GetBaggageStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageStatusesQuery, IReadOnlyList<BaggageStatusEntity>>
{
    public Task<IReadOnlyList<BaggageStatusEntity>> Handle(GetBaggageStatusesQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageStatusEntity>().GetAllAsync(ct);
    }
}