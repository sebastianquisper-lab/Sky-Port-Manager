using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;

namespace Sky.Application.UseCases.AircraftModel.Commands;

public sealed record UpdateAircraftModelCommand(
    int ModelId,
    int ManufacturerId,
    string ModelName,
    string? Description) : IRequest<AircraftModelEntity?>;

internal sealed class UpdateAircraftModelHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftModelCommand, AircraftModelEntity?>
{
    public async Task<AircraftModelEntity?> Handle(UpdateAircraftModelCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftModelEntity>().GetByIdAsync(request.ModelId, ct);
        if (entity is null) return null;

        entity.ManufacturerId = request.ManufacturerId;
        entity.ModelName = request.ModelName;
        entity.Description = request.Description;

        uow.Repository<AircraftModelEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
