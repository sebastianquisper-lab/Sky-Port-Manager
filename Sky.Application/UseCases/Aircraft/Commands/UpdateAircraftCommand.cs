using MediatR;
using Sky.Domain.Ports;
using AircraftEntity = Sky.Domain.Entities.Aircraft;

namespace Sky.Application.UseCases.Aircraft.Commands;

public sealed record UpdateAircraftCommand(
    int AircraftId,
    int AirlineId,
    int ModelId,
    int AircraftStatusId,
    string RegistrationCode,
    string? SerialNumber,
    short SeatCapacity,
    short? ManufactureYear,
    bool IsActive) : IRequest<AircraftEntity?>;

internal sealed class UpdateAircraftHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateAircraftCommand, AircraftEntity?>
{
    public async Task<AircraftEntity?> Handle(UpdateAircraftCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftEntity>().GetByIdAsync(request.AircraftId, ct);
        if (entity is null) return null;

        entity.AirlineId = request.AirlineId;
        entity.ModelId = request.ModelId;
        entity.AircraftStatusId = request.AircraftStatusId;
        entity.RegistrationCode = request.RegistrationCode;
        entity.SerialNumber = request.SerialNumber;
        entity.SeatCapacity = request.SeatCapacity;
        entity.ManufactureYear = request.ManufactureYear;
        entity.IsActive = request.IsActive;

        uow.Repository<AircraftEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
