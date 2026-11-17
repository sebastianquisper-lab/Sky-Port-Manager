using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;

namespace Sky.Application.UseCases.AircraftManufacturer.Commands;

public sealed record UpdateAircraftManufacturerCommand(
    int ManufacturerId,
    string ManufacturerName) : IRequest<AircraftManufacturerEntity?>;

internal sealed class UpdateAircraftManufacturerHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftManufacturerCommand, AircraftManufacturerEntity?>
{
    public async Task<AircraftManufacturerEntity?> Handle(UpdateAircraftManufacturerCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftManufacturerEntity>().GetByIdAsync(request.ManufacturerId, ct);
        if (entity is null) return null;

        entity.ManufacturerName = request.ManufacturerName;

        uow.Repository<AircraftManufacturerEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
