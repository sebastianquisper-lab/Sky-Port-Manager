namespace Sky.Application.UseCases.AircraftModel.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;


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
        var aircraftModel = await uow.Repository<AircraftModelEntity>().GetByIdAsync(request.ModelId, ct);
        if (aircraftModel is null) return null;

        aircraftModel.ManufacturerId = request.ManufacturerId;
        aircraftModel.ModelName = request.ModelName;
        aircraftModel.Description = request.Description;

        uow.Repository<AircraftModelEntity>().Update(aircraftModel);
        await uow.SaveChangesAsync(ct);

        return aircraftModel;
    }
}