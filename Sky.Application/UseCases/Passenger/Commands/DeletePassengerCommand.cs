using MediatR;
using Sky.Domain.Ports;
using PassengerEntity = Sky.Domain.Entities.Passenger;

namespace Sky.Application.UseCases.Passenger.Commands;

public sealed record DeletePassengerCommand(
    int PassengerId) : IRequest<bool>;

internal sealed class DeletePassengerHandler(IUnitOfWork uow)
    : IRequestHandler<DeletePassengerCommand, bool>
{
    public async Task<bool> Handle(DeletePassengerCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PassengerEntity>().GetByIdAsync(request.PassengerId, ct);
        if (entity is null) return false;

        uow.Repository<PassengerEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
