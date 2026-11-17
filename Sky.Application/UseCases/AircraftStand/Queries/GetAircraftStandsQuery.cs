namespace Sky.Application.UseCases.AircraftStand.Queries;

using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;


public sealed record GetAircraftStandsQuery : IRequest<List<AircraftStandEntity>>;

internal sealed class GetAircraftStandsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAircraftStandsQuery, List<AircraftStandEntity>>
{
    public Task<List<AircraftStandEntity>> Handle(GetAircraftStandsQuery request, CancellationToken ct)
    {
        return uow.Repository<AircraftStandEntity>().GetAllAsync(ct);
    }
}