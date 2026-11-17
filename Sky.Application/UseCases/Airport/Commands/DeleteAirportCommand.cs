namespace Sky.Application.UseCases.Airport.Commands;

using MediatR;
using Sky.Domain.Ports;
using AirportEntity = Sky.Domain.Entities.Airport;


public sealed record DeleteAirportCommand(int AirportId) : IRequest<bool>;

internal sealed class DeleteAirportHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteAirportCommand, bool>
{
    public async Task<bool> Handle(DeleteAirportCommand request, CancellationToken ct)
    {
        var airport = await uow.Repository<AirportEntity>().GetByIdAsync(request.AirportId, ct);
        if (airport is null) return false;

        uow.Repository<AirportEntity>().Delete(airport);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}