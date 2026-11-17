using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;

namespace Sky.Application.UseCases.AircraftManufacturer.Commands;

public sealed record DeleteAircraftManufacturerCommand(
    int ManufacturerId) : IRequest<bool>;

internal sealed class DeleteAircraftManufacturerHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftManufacturerCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftManufacturerCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftManufacturerEntity>().GetByIdAsync(request.ManufacturerId, ct);
        if (entity is null) return false;

        uow.Repository<AircraftManufacturerEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
