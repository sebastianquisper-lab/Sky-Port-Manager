using MediatR;
using Sky.Domain.Ports;
using AircraftModelEntity = Sky.Domain.Entities.AircraftModel;

namespace Sky.Application.UseCases.AircraftModel.Commands;

public sealed record DeleteAircraftModelCommand(
    int ModelId) : IRequest<bool>;

internal sealed class DeleteAircraftModelHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAircraftModelCommand, bool>
{
    public async Task<bool> Handle(DeleteAircraftModelCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AircraftModelEntity>().GetByIdAsync(request.ModelId, ct);
        if (entity is null) return false;

        uow.Repository<AircraftModelEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
