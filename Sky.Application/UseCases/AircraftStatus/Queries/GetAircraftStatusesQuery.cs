namespace Sky.Application.UseCases.AircraftStatus.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;


public sealed record GetAircraftStatusesQuery : IRequest<List<AircraftStatusEntity>>;

internal sealed class GetAircraftStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftStatusesQuery, List<AircraftStatusEntity>>
{
    public Task<List<AircraftStatusEntity>> Handle(GetAircraftStatusesQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftStatusEntity>().GetAllAsync(ct);
    }
}