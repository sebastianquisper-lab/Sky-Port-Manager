namespace Sky.Application.UseCases.CheckInCounter.Commands;

using MediatR;
using Sky.Domain.Ports;
using CheckInCounterEntity = Sky.Domain.Entities.CheckInCounter;


public sealed record DeleteCheckInCounterCommand(int CounterId) : IRequest<bool>;

internal sealed class DeleteCheckInCounterHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteCheckInCounterCommand, bool>
{
    public async Task<bool> Handle(DeleteCheckInCounterCommand request, CancellationToken ct)
    {
        var checkInCounter = await uow.Repository<CheckInCounterEntity>().GetByIdAsync(request.CounterId, ct);
        if (checkInCounter is null) return false;

        uow.Repository<CheckInCounterEntity>().Delete(checkInCounter);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}