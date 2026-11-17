namespace Sky.Application.UseCases.AircraftManufacturer.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;


public sealed record UpdateAircraftManufacturerCommand(
    int ManufacturerId,
    string ManufacturerName) : IRequest<AircraftManufacturerEntity?>;

internal sealed class UpdateAircraftManufacturerHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftManufacturerCommand, AircraftManufacturerEntity?>
{
    public async Task<AircraftManufacturerEntity?> Handle(UpdateAircraftManufacturerCommand request, CancellationToken ct)
    {
        var aircraftManufacturer = await uow.Repository<AircraftManufacturerEntity>().GetByIdAsync(request.ManufacturerId, ct);
        if (aircraftManufacturer is null) return null;

        aircraftManufacturer.ManufacturerName = request.ManufacturerName;

        uow.Repository<AircraftManufacturerEntity>().Update(aircraftManufacturer);
        await uow.SaveChangesAsync(ct);

        return aircraftManufacturer;
    }
}