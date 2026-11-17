namespace Sky.Application.UseCases.Aircraft.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;


public sealed record GetAircraftsQuery : IRequest<List<AircraftEntity>>;

internal sealed class GetAircraftsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftsQuery, List<AircraftEntity>>
{
    public Task<List<AircraftEntity>> Handle(GetAircraftsQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftEntity>().GetAllAsync(ct);
    }
}