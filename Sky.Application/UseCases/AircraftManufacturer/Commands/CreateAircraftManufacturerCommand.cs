using MediatR;
using Sky.Domain.Ports;
using AircraftManufacturerEntity = Sky.Domain.Entities.AircraftManufacturer;

namespace Sky.Application.UseCases.AircraftManufacturer.Commands;

public sealed record CreateAircraftManufacturerCommand(
    string ManufacturerName) : IRequest<AircraftManufacturerEntity>;

internal sealed class CreateAircraftManufacturerHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftManufacturerCommand, AircraftManufacturerEntity>
{
    public async Task<AircraftManufacturerEntity> Handle(CreateAircraftManufacturerCommand request, CancellationToken ct)
    {
        var entity = new AircraftManufacturerEntity
        {
            ManufacturerName = request.ManufacturerName
        };

        await uow.Repository<AircraftManufacturerEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
