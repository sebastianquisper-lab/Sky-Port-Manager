using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;

namespace Sky.Application.UseCases.AircraftStand.Queries;

public sealed record GetAircraftStandByIdQuery(
    int StandId) : IRequest<AircraftStandEntity?>;

internal sealed class GetAircraftStandByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftStandByIdQuery, AircraftStandEntity?>
{
    public Task<AircraftStandEntity?> Handle(GetAircraftStandByIdQuery request, CancellationToken ct)
        => uow.Repository<AircraftStandEntity>().GetByIdAsync(request.StandId, ct);
}
