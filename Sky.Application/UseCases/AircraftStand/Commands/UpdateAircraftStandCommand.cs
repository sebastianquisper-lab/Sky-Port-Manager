using MediatR;
using Sky.Domain.Ports;
using AircraftStandEntity = Sky.Domain.Entities.AircraftStand;

namespace Sky.Application.UseCases.AircraftStand.Commands;

public sealed record UpdateAircraftStandCommand(
    int StandId,
    int AirportId,
    string StandCode,
    bool IsRemote,
    bool IsActive) : IRequest<AircraftStandEntity?>;

internal sealed class UpdateAircraftStandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftStandCommand, AircraftStandEntity?>
{
    public async Task<AircraftStandEntity?> Handle(UpdateAircraftStandCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftStandEntity>().GetByIdAsync(request.StandId, ct);
        if (entity is null) return null;

        entity.AirportId = request.AirportId;
        entity.StandCode = request.StandCode;
        entity.IsRemote = request.IsRemote;
        entity.IsActive = request.IsActive;

        uow.Repository<AircraftStandEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
