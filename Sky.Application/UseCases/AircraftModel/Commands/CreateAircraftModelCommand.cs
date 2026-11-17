using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;

namespace Sky.Application.UseCases.AircraftModel.Commands;

public sealed record CreateAircraftModelCommand(
    int ManufacturerId,
    string ModelName,
    string? Description) : IRequest<AircraftModelEntity>;

internal sealed class CreateAircraftModelHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAircraftModelCommand, AircraftModelEntity>
{
    public async Task<AircraftModelEntity> Handle(CreateAircraftModelCommand request, CancellationToken ct)
    {
        var entity = new AircraftModelEntity
        {
            ManufacturerId = request.ManufacturerId,
            ModelName = request.ModelName,
            Description = request.Description
        };

        await uow.Repository<AircraftModelEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
