namespace Sky.Application.UseCases.Baggage.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;


public sealed record GetBaggagesQuery : IRequest<List<BaggageEntity>>;

internal sealed class GetBaggagesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggagesQuery, List<BaggageEntity>>
{
    public Task<List<BaggageEntity>> Handle(GetBaggagesQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageEntity>().GetAllAsync(ct);
    }
}