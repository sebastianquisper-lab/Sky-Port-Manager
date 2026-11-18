using MediatR;
using Sky.Domain.Ports;
using FlightCrewAssignmentEntity = Sky.Domain.Entities.FlightCrewAssignment;

namespace Sky.Application.UseCases.FlightCrewAssignment.Queries;

public sealed record GetFlightCrewAssignmentByIdQuery(
    int FlightId,
    int EmployeeId) : IRequest<FlightCrewAssignmentEntity?>;

internal sealed class GetFlightCrewAssignmentByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightCrewAssignmentByIdQuery, FlightCrewAssignmentEntity?>
{
    public Task<FlightCrewAssignmentEntity?> Handle(GetFlightCrewAssignmentByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightCrewAssignmentEntity>().FirstOrDefaultAsync(x => x.FlightId == request.FlightId && x.EmployeeId == request.EmployeeId, ct);
}
