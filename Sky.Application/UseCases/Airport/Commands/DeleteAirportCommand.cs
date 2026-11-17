using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;

namespace Sky.Application.UseCases.Airport.Commands;

public sealed record DeleteAirportCommand(
    int AirportId) : IRequest<bool>;

internal sealed class DeleteAirportHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAirportCommand, bool>
{
    public async Task<bool> Handle(DeleteAirportCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AirportEntity>().GetByIdAsync(request.AirportId, ct);
        if (entity is null) return false;

        uow.Repository<AirportEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
