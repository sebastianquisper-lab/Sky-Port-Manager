using MediatR;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;

namespace Sky.Application.UseCases.Airline.Commands;

public sealed record DeleteAirlineCommand(
    int AirlineId) : IRequest<bool>;

internal sealed class DeleteAirlineHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAirlineCommand, bool>
{
    public async Task<bool> Handle(DeleteAirlineCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<AirlineEntity>().GetByIdAsync(request.AirlineId, ct);
        if (entity is null) return false;

        uow.Repository<AirlineEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
