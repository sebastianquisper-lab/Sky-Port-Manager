using MediatR;
using Sky.Domain.Ports;
using FlightStatusEntity = Sky.Domain.Entities.FlightStatus;

namespace Sky.Application.UseCases.FlightStatus.Commands;

public sealed record DeleteFlightStatusCommand(
    int FlightStatusId) : IRequest<bool>;

internal sealed class DeleteFlightStatusHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFlightStatusCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightStatusEntity>().GetByIdAsync(request.FlightStatusId, ct);
        if (entity is null) return false;

        uow.Repository<FlightStatusEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
