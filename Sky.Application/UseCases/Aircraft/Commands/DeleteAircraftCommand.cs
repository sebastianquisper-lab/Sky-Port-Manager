namespace Sky.Application.UseCases.Aircraft.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;


public sealed record DeleteAircraftCommand(int AircraftId) : IRequest<bool>;

internal sealed class DeleteAircraftHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftCommand request, CancellationToken ct)
    {
        var aircraft = await uow.Repository<AircraftEntity>().GetByIdAsync(request.AircraftId, ct);
        if (aircraft is null) return false;

        uow.Repository<AircraftEntity>().Delete(aircraft);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}