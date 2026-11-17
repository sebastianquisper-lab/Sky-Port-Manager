using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;

namespace Sky.Application.UseCases.AircraftStand.Commands;

public sealed record DeleteAircraftStandCommand(
    int StandId) : IRequest<bool>;

internal sealed class DeleteAircraftStandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftStandCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftStandCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftStandEntity>().GetByIdAsync(request.StandId, ct);
        if (entity is null) return false;

        uow.Repository<AircraftStandEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
