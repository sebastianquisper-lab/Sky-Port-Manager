using MediatR;
using Sky.Domain.Ports;
using RunwayEntity = Sky.Domain.Entities.Runway;

namespace Sky.Application.UseCases.Runway.Commands;

public sealed record UpdateRunwayCommand(
    int RunwayId,
    int AirportId,
    string RunwayCode,
    int LengthM,
    string? SurfaceType,
    bool IsActive) : IRequest<RunwayEntity?>;

internal sealed class UpdateRunwayHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateRunwayCommand, RunwayEntity?>
{
    public async Task<RunwayEntity?> Handle(UpdateRunwayCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<RunwayEntity>().GetByIdAsync(request.RunwayId, ct);
        if (entity is null) return null;

        entity.AirportId = request.AirportId;
        entity.RunwayCode = request.RunwayCode;
        entity.LengthM = request.LengthM;
        entity.SurfaceType = request.SurfaceType;
        entity.IsActive = request.IsActive;

        uow.Repository<RunwayEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
