namespace Sky.Application.UseCases.AircraftManufacturer.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;


public sealed record DeleteAircraftManufacturerCommand(int ManufacturerId) : IRequest<bool>;

internal sealed class DeleteAircraftManufacturerHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftManufacturerCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftManufacturerCommand request, CancellationToken ct)
    {
        var aircraftManufacturer = await uow.Repository<AircraftManufacturerEntity>().GetByIdAsync(request.ManufacturerId, ct);
        if (aircraftManufacturer is null) return false;

        uow.Repository<AircraftManufacturerEntity>().Delete(aircraftManufacturer);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}