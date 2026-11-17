using MediatR;
using Sky.Domain.Ports;
using AircraftStatusEntity = Sky.Domain.Entities.AircraftStatus;

namespace Sky.Application.UseCases.AircraftStatus.Commands;

public sealed record UpdateAircraftStatusCommand(
    int AircraftStatusId,
    string StatusName,
    string? Description) : IRequest<AircraftStatusEntity?>;

internal sealed class UpdateAircraftStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftStatusCommand, AircraftStatusEntity?>
{
    public async Task<AircraftStatusEntity?> Handle(UpdateAircraftStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftStatusEntity>().GetByIdAsync(request.AircraftStatusId, ct);
        if (entity is null) return null;

        entity.StatusName = request.StatusName;
        entity.Description = request.Description;

        uow.Repository<AircraftStatusEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
