namespace Sky.Application.UseCases.BaggageType.Queries;

using MediatR;
using Sky.Domain.Ports;
using BaggageTypeEntity = Sky.Domain.Entities.BaggageType;


public sealed record GetBaggageTypesQuery : IRequest<List<BaggageTypeEntity>>;

internal sealed class GetBaggageTypesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBaggageTypesQuery, List<BaggageTypeEntity>>
{
    public Task<List<BaggageTypeEntity>> Handle(GetBaggageTypesQuery request, CancellationToken ct)
    {
        return uow.Repository<BaggageTypeEntity>().GetAllAsync(ct);
    }
}