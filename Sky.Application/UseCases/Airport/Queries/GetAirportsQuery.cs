using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;

namespace Sky.Application.UseCases.Airport.Queries;

public sealed record GetAirportsQuery : IRequest<IReadOnlyList<AirportEntity>>;

internal sealed class GetAirportsHandler(IUnitOfWork uow)
    : IRequestHandler<GetAirportsQuery, IReadOnlyList<AirportEntity>>
{
    public Task<IReadOnlyList<AirportEntity>> Handle(GetAirportsQuery request, CancellationToken ct)
        => uow.Repository<AirportEntity>().GetAllAsync(ct);
}
