namespace Sky.Application.UseCases.CheckInCounter.Commands;

using MediatR;
using Sky.Domain.Ports;
using CheckInCounterEntity = Sky.Domain.Entities.CheckInCounter;


public sealed record CreateCheckInCounterCommand(
    int TerminalId,
    string CounterCode) : IRequest<CheckInCounterEntity>;

internal sealed class CreateCheckInCounterHandler(IUnitOfWork uow)
    : IRequestHandler<CreateCheckInCounterCommand, CheckInCounterEntity>
{
    public async Task<CheckInCounterEntity> Handle(CreateCheckInCounterCommand request, CancellationToken ct)
    {
        var checkInCounter = new CheckInCounterEntity
        {
            TerminalId = request.TerminalId,
            CounterCode = request.CounterCode,
            IsActive = true
        };

        await uow.Repository<CheckInCounterEntity>().AddAsync(checkInCounter, ct);
        await uow.SaveChangesAsync(ct);

        return checkInCounter;
    }
}