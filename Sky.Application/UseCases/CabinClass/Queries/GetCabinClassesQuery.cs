namespace Sky.Application.UseCases.CabinClass.Queries;

using MediatR;
using Sky.Domain.Ports;
using CabinClassEntity = Sky.Domain.Entities.CabinClass;


public sealed record GetCabinClassesQuery : IRequest<IReadOnlyList<CabinClassEntity>>;

internal sealed class GetCabinClassesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCabinClassesQuery, IReadOnlyList<CabinClassEntity>>
{
    public Task<IReadOnlyList<CabinClassEntity>> Handle(GetCabinClassesQuery request, CancellationToken ct)
    {
        return uow.Repository<CabinClassEntity>().GetAllAsync(ct);
    }
}