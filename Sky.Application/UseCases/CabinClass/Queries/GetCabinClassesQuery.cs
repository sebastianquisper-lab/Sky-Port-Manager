namespace Sky.Application.UseCases.CabinClass.Queries;

using MediatR;
using Sky.Domain.Ports;
using CabinClassEntity = Sky.Domain.Entities.CabinClass;


public sealed record GetCabinClassesQuery : IRequest<List<CabinClassEntity>>;

internal sealed class GetCabinClassesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCabinClassesQuery, List<CabinClassEntity>>
{
    public Task<List<CabinClassEntity>> Handle(GetCabinClassesQuery request, CancellationToken ct)
    {
        return uow.Repository<CabinClassEntity>().GetAllAsync(ct);
    }
}