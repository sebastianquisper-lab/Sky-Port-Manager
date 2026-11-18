using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using FlightCrewAssignmentEntity = Sky.Domain.Entities.FlightCrewAssignment;

namespace Sky.Application.UseCases.FlightCrewAssignment.Commands;

public sealed record CreateFlightCrewAssignmentCommand(
    int FlightId,
    int EmployeeId,
    string CrewPosition) : IRequest<FlightCrewAssignmentEntity>;

internal sealed class CreateFlightCrewAssignmentHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock)
    : IRequestHandler<CreateFlightCrewAssignmentCommand, FlightCrewAssignmentEntity>
{
    public async Task<FlightCrewAssignmentEntity> Handle(CreateFlightCrewAssignmentCommand request, CancellationToken ct)
    {
        var entity = new FlightCrewAssignmentEntity
        {
            FlightId = request.FlightId,
            EmployeeId = request.EmployeeId,
            CrewPosition = request.CrewPosition,
            AssignedAtUtc = clock.UtcNow
        };

        await uow.Repository<FlightCrewAssignmentEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
