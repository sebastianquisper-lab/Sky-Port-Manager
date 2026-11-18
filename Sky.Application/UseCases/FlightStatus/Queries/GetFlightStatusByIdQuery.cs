using MediatR;
using Sky.Domain.Ports;
using FlightStatusEntity = Sky.Domain.Entities.FlightStatus;

namespace Sky.Application.UseCases.FlightStatus.Queries;

public sealed record GetFlightStatusByIdQuery(
    int FlightStatusId) : IRequest<FlightStatusEntity?>;

internal sealed class GetFlightStatusByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightStatusByIdQuery, FlightStatusEntity?>
{
    public Task<FlightStatusEntity?> Handle(GetFlightStatusByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightStatusEntity>().GetByIdAsync(request.FlightStatusId, ct);
}
