namespace Sky.Application.UseCases.CabinClass.Queries;

using MediatR;
using Sky.Domain.Ports;
using CabinClassEntity = Sky.Domain.Entities.CabinClass;


public sealed record GetCabinClassByIdQuery(int CabinClassId) : IRequest<CabinClassEntity?>;

internal sealed class GetCabinClassByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetCabinClassByIdQuery, CabinClassEntity?>
{
    public Task<CabinClassEntity?> Handle(GetCabinClassByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<CabinClassEntity>().GetByIdAsync(request.CabinClassId, ct);
    }
}