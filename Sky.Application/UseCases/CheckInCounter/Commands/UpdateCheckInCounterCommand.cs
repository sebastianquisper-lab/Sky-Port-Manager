namespace Sky.Application.UseCases.CheckInCounter.Commands;

using MediatR;
using Sky.Domain.Ports;
using CheckInCounterEntity = Sky.Domain.Entities.CheckInCounter;


public sealed record UpdateCheckInCounterCommand(
    int CounterId,
    int TerminalId,
    string CounterCode,
    bool IsActive) : IRequest<CheckInCounterEntity?>;

internal sealed class UpdateCheckInCounterHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateCheckInCounterCommand, CheckInCounterEntity?>
{
    public async Task<CheckInCounterEntity?> Handle(UpdateCheckInCounterCommand request, CancellationToken ct)
    {
        var checkInCounter = await uow.Repository<CheckInCounterEntity>().GetByIdAsync(request.CounterId, ct);
        if (checkInCounter is null) return null;

        checkInCounter.TerminalId = request.TerminalId;
        checkInCounter.CounterCode = request.CounterCode;
        checkInCounter.IsActive = request.IsActive;

        uow.Repository<CheckInCounterEntity>().Update(checkInCounter);
        await uow.SaveChangesAsync(ct);

        return checkInCounter;
    }
}