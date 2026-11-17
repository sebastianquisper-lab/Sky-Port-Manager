namespace Sky.Application.UseCases.AircraftModel.Commands;

using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;


public sealed record DeleteAircraftModelCommand(int ModelId) : IRequest<bool>;

internal sealed class DeleteAircraftModelHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftModelCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftModelCommand request, CancellationToken ct)
    {
        var aircraftModel = await uow.Repository<AircraftModelEntity>().GetByIdAsync(request.ModelId, ct);
        if (aircraftModel is null) return false;

        uow.Repository<AircraftModelEntity>().Delete(aircraftModel);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}