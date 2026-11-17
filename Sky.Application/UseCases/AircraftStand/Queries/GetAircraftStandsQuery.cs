using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;

namespace Sky.Application.UseCases.AircraftStand.Queries;

public sealed record GetAircraftStandsQuery : IRequest<IReadOnlyList<AircraftStandEntity>>;

internal sealed class GetAircraftStandsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftStandsQuery, IReadOnlyList<AircraftStandEntity>>
{
    public Task<IReadOnlyList<AircraftStandEntity>> Handle(GetAircraftStandsQuery request, CancellationToken ct)
        => uow.Repository<AircraftStandEntity>().GetAllAsync(ct);
}
