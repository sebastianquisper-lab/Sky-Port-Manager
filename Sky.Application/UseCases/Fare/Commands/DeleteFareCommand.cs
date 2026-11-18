using MediatR;
using Sky.Domain.Ports;
using FareEntity = Sky.Domain.Entities.Fare;

namespace Sky.Application.UseCases.Fare.Commands;

public sealed record DeleteFareCommand(
    int FareId) : IRequest<bool>;

internal sealed class DeleteFareHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteFareCommand, bool>
{
    public async Task<bool> Handle(DeleteFareCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FareEntity>().GetByIdAsync(request.FareId, ct);
        if (entity is null) return false;

        uow.Repository<FareEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
