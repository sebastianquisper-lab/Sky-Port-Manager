using MediatR;
using Sky.Domain.Ports;
using BaggageEntity = Sky.Domain.Entities.Baggage;

namespace Sky.Application.UseCases.Baggage.Queries;

public sealed record GetBaggageByIdQuery(
    int BaggageId) : IRequest<BaggageEntity?>;

internal sealed class GetBaggageByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageByIdQuery, BaggageEntity?>
{
    public Task<BaggageEntity?> Handle(GetBaggageByIdQuery request, CancellationToken ct)
        => uow.Repository<BaggageEntity>().GetByIdAsync(request.BaggageId, ct);
}
