using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;

namespace Sky.Application.UseCases.AircraftModel.Queries;

public sealed record GetAircraftModelsQuery : IRequest<IReadOnlyList<AircraftModelEntity>>;

internal sealed class GetAircraftModelsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftModelsQuery, IReadOnlyList<AircraftModelEntity>>
{
    public Task<IReadOnlyList<AircraftModelEntity>> Handle(GetAircraftModelsQuery request, CancellationToken ct)
        => uow.Repository<AircraftModelEntity>().GetAllAsync(ct);
}
