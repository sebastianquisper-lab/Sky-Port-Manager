namespace Sky.Application.UseCases.AircraftModel.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;


public sealed record CreateAircraftModelCommand(
    int ManufacturerId,
    string ModelName,
    string? Description) : IRequest<AircraftModelEntity>;

internal sealed class CreateAircraftModelHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftModelCommand, AircraftModelEntity>
{
    public async Task<AircraftModelEntity> Handle(CreateAircraftModelCommand request, CancellationToken ct)
    {
        var aircraftModel = new AircraftModelEntity
        {
            ManufacturerId = request.ManufacturerId,
            ModelName = request.ModelName,
            Description = request.Description
        };

        await uow.Repository<AircraftModelEntity>().AddAsync(aircraftModel, ct);
        await uow.SaveChangesAsync(ct);

        return aircraftModel;
    }
}