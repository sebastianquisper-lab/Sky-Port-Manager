namespace Sky.Application.UseCases.AircraftManufacturer.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;


public sealed record CreateAircraftManufacturerCommand(string ManufacturerName) : IRequest<AircraftManufacturerEntity>;

internal sealed class CreateAircraftManufacturerHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftManufacturerCommand, AircraftManufacturerEntity>
{
    public async Task<AircraftManufacturerEntity> Handle(CreateAircraftManufacturerCommand request, CancellationToken ct)
    {
        var aircraftManufacturer = new AircraftManufacturerEntity
        {
            ManufacturerName = request.ManufacturerName
        };

        await uow.Repository<AircraftManufacturerEntity>().AddAsync(aircraftManufacturer, ct);
        await uow.SaveChangesAsync(ct);

        return aircraftManufacturer;
    }
}