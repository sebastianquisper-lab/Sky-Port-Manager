namespace Sky.Application.UseCases.BaggageStatus.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageStatusEntity = Sky.Domain.Entities.BaggageStatus;


public sealed record GetBaggageStatusesQuery : IRequest<List<BaggageStatusEntity>>;

internal sealed class GetBaggageStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageStatusesQuery, List<BaggageStatusEntity>>
{
    public Task<List<BaggageStatusEntity>> Handle(GetBaggageStatusesQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageStatusEntity>().GetAllAsync(ct);
    }
}