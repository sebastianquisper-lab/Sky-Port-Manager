using MediatR;
using Sky.Domain.Ports;
using FlightStandAssignmentEntity = Sky.Domain.Entities.FlightStandAssignment;

namespace Sky.Application.UseCases.FlightStandAssignment.Queries;

public sealed record GetFlightStandAssignmentsQuery : IRequest<IReadOnlyList<FlightStandAssignmentEntity>>;

internal sealed class GetFlightStandAssignmentsHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightStandAssignmentsQuery, IReadOnlyList<FlightStandAssignmentEntity>>
{
    public Task<IReadOnlyList<FlightStandAssignmentEntity>> Handle(GetFlightStandAssignmentsQuery request, CancellationToken ct)
        => uow.Repository<FlightStandAssignmentEntity>().GetAllAsync(ct);
}
