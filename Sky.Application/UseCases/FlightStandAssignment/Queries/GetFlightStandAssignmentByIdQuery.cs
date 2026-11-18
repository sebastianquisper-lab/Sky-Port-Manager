using MediatR;
using Sky.Domain.Ports;
using FlightStandAssignmentEntity = Sky.Domain.Entities.FlightStandAssignment;

namespace Sky.Application.UseCases.FlightStandAssignment.Queries;

public sealed record GetFlightStandAssignmentByIdQuery(
    int AssignmentId) : IRequest<FlightStandAssignmentEntity?>;

internal sealed class GetFlightStandAssignmentByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightStandAssignmentByIdQuery, FlightStandAssignmentEntity?>
{
    public Task<FlightStandAssignmentEntity?> Handle(GetFlightStandAssignmentByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightStandAssignmentEntity>().GetByIdAsync(request.AssignmentId, ct);
}
