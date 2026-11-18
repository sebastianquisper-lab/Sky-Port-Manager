using MediatR;
using Sky.Domain.Ports;
using FlightCrewAssignmentEntity = Sky.Domain.Entities.FlightCrewAssignment;

namespace Sky.Application.UseCases.FlightCrewAssignment.Commands;

public sealed record UpdateFlightCrewAssignmentCommand(
    int FlightId,
    int EmployeeId,
    string CrewPosition) : IRequest<FlightCrewAssignmentEntity?>;

internal sealed class UpdateFlightCrewAssignmentHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFlightCrewAssignmentCommand, FlightCrewAssignmentEntity?>
{
    public async Task<FlightCrewAssignmentEntity?> Handle(UpdateFlightCrewAssignmentCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightCrewAssignmentEntity>().FirstOrDefaultAsync(x => x.FlightId == request.FlightId && x.EmployeeId == request.EmployeeId, ct);
        if (entity is null) return null;

        entity.CrewPosition = request.CrewPosition;

        uow.Repository<FlightCrewAssignmentEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
