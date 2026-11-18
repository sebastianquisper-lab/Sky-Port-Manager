using MediatR;
using Sky.Domain.Ports;
using FlightCrewAssignmentEntity = Sky.Domain.Entities.FlightCrewAssignment;

namespace Sky.Application.UseCases.FlightCrewAssignment.Commands;

public sealed record DeleteFlightCrewAssignmentCommand(
    int FlightId,
    int EmployeeId) : IRequest<bool>;

internal sealed class DeleteFlightCrewAssignmentHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFlightCrewAssignmentCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightCrewAssignmentCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightCrewAssignmentEntity>().FirstOrDefaultAsync(x => x.FlightId == request.FlightId && x.EmployeeId == request.EmployeeId, ct);
        if (entity is null) return false;

        uow.Repository<FlightCrewAssignmentEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
