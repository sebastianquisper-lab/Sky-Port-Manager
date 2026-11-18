using MediatR;
using Sky.Domain.Ports;
using FlightStandAssignmentEntity = Sky.Domain.Entities.FlightStandAssignment;

namespace Sky.Application.UseCases.FlightStandAssignment.Commands;

public sealed record DeleteFlightStandAssignmentCommand(
    int AssignmentId) : IRequest<bool>;

internal sealed class DeleteFlightStandAssignmentHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFlightStandAssignmentCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightStandAssignmentCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightStandAssignmentEntity>().GetByIdAsync(request.AssignmentId, ct);
        if (entity is null) return false;

        uow.Repository<FlightStandAssignmentEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
