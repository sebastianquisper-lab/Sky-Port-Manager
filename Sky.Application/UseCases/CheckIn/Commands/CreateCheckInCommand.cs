namespace Sky.Application.UseCases.CheckIn.Commands;

using MediatR;
using Sky.Domain.Ports;
using CheckInEntity = Sky.Domain.Entities.CheckIn;


public sealed record CreateCheckInCommand(
    int TicketId,
    int? CounterId,
    int? EmployeeId,
    string Channel) : IRequest<CheckInEntity>;

internal sealed class CreateCheckInHandler(IUnitOfWork uow)
    : IRequestHandler<CreateCheckInCommand, CheckInEntity>
{
    public async Task<CheckInEntity> Handle(CreateCheckInCommand request, CancellationToken ct)
    {
        var checkIn = new CheckInEntity
        {
            TicketId = request.TicketId,
            CounterId = request.CounterId,
            EmployeeId = request.EmployeeId,
            Channel = request.Channel,
            CheckInTimeUtc = DateTime.UtcNow
        };

        await uow.Repository<CheckInEntity>().AddAsync(checkIn, ct);
        await uow.SaveChangesAsync(ct);

        return checkIn;
    }
}