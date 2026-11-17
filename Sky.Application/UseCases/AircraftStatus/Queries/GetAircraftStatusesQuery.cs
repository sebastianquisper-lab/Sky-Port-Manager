using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;

namespace Sky.Application.UseCases.AircraftStatus.Queries;

public sealed record GetAircraftStatusesQuery : IRequest<IReadOnlyList<AircraftStatusEntity>>;

internal sealed class GetAircraftStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftStatusesQuery, IReadOnlyList<AircraftStatusEntity>>
{
    public Task<IReadOnlyList<AircraftStatusEntity>> Handle(GetAircraftStatusesQuery request, CancellationToken ct)
        => uow.Repository<AircraftStatusEntity>().GetAllAsync(ct);
}
