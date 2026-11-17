using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;

namespace Sky.Application.UseCases.Aircraft.Queries;

public sealed record GetAircraftsQuery : IRequest<IReadOnlyList<AircraftEntity>>;

internal sealed class GetAircraftsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftsQuery, IReadOnlyList<AircraftEntity>>
{
    public Task<IReadOnlyList<AircraftEntity>> Handle(GetAircraftsQuery request, CancellationToken ct)
        => uow.Repository<AircraftEntity>().GetAllAsync(ct);
}
