using MediatR;
using Sky.Domain.Ports;
using FlightStatusEntity = Sky.Domain.Entities.FlightStatus;

namespace Sky.Application.UseCases.FlightStatus.Queries;

public sealed record GetFlightStatusesQuery : IRequest<IReadOnlyList<FlightStatusEntity>>;

internal sealed class GetFlightStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightStatusesQuery, IReadOnlyList<FlightStatusEntity>>
{
    public Task<IReadOnlyList<FlightStatusEntity>> Handle(GetFlightStatusesQuery request, CancellationToken ct)
        => uow.Repository<FlightStatusEntity>().GetAllAsync(ct);
}
