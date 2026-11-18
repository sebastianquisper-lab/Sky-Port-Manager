using MediatR;
using Sky.Domain.Ports;
using FlightCrewAssignmentEntity = Sky.Domain.Entities.FlightCrewAssignment;

namespace Sky.Application.UseCases.FlightCrewAssignment.Queries;

public sealed record GetFlightCrewAssignmentsQuery : IRequest<IReadOnlyList<FlightCrewAssignmentEntity>>;

internal sealed class GetFlightCrewAssignmentsHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightCrewAssignmentsQuery, IReadOnlyList<FlightCrewAssignmentEntity>>
{
    public Task<IReadOnlyList<FlightCrewAssignmentEntity>> Handle(GetFlightCrewAssignmentsQuery request, CancellationToken ct)
        => uow.Repository<FlightCrewAssignmentEntity>().GetAllAsync(ct);
}
