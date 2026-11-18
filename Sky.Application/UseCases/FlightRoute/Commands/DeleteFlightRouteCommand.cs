using MediatR;
using Sky.Domain.Ports;
using FlightRouteEntity = Sky.Domain.Entities.FlightRoute;

namespace Sky.Application.UseCases.FlightRoute.Commands;

public sealed record DeleteFlightRouteCommand(
    int RouteId) : IRequest<bool>;

internal sealed class DeleteFlightRouteHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFlightRouteCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightRouteCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightRouteEntity>().GetByIdAsync(request.RouteId, ct);
        if (entity is null) return false;

        uow.Repository<FlightRouteEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
