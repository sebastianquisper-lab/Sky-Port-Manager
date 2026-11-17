using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;

namespace Sky.Application.UseCases.Baggage.Queries;

public sealed record GetBaggagesQuery : IRequest<IReadOnlyList<BaggageEntity>>;

internal sealed class GetBaggagesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggagesQuery, IReadOnlyList<BaggageEntity>>
{
    public Task<IReadOnlyList<BaggageEntity>> Handle(GetBaggagesQuery request, CancellationToken ct)
        => uow.Repository<BaggageEntity>().GetAllAsync(ct);
}
