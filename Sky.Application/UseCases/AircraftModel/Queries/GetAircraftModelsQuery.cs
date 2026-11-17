namespace Sky.Application.UseCases.AircraftModel.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;


public sealed record GetAircraftModelsQuery : IRequest<List<AircraftModelEntity>>;

internal sealed class GetAircraftModelsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftModelsQuery, List<AircraftModelEntity>>
{
    public Task<List<AircraftModelEntity>> Handle(GetAircraftModelsQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftModelEntity>().GetAllAsync(ct);
    }
}