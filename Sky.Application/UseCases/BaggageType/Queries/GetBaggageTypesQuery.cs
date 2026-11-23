namespace Sky.Application.UseCases.BaggageType.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageTypeEntity = Sky.Domain.Entities.BaggageType;


public sealed record GetBaggageTypesQuery : IRequest<IReadOnlyList<BaggageTypeEntity>>;

internal sealed class GetBaggageTypesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageTypesQuery, IReadOnlyList<BaggageTypeEntity>>
{
    public Task<IReadOnlyList<BaggageTypeEntity>> Handle(GetBaggageTypesQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageTypeEntity>().GetAllAsync(ct);
    }
}