using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;

namespace Sky.Application.UseCases.AircraftStatus.Queries;

public sealed record GetAircraftStatusByIdQuery(
    int AircraftStatusId) : IRequest<AircraftStatusEntity?>;

internal sealed class GetAircraftStatusByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftStatusByIdQuery, AircraftStatusEntity?>
{
    public Task<AircraftStatusEntity?> Handle(GetAircraftStatusByIdQuery request, CancellationToken ct)
        => uow.Repository<AircraftStatusEntity>().GetByIdAsync(request.AircraftStatusId, ct);
}
