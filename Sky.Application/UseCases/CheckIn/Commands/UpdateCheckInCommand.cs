namespace Sky.Application.UseCases.CheckIn.Commands;

using MediatR;
using Sky.Domain.Ports;
using CheckInEntity = Sky.Domain.Entities.CheckIn;


public sealed record UpdateCheckInCommand(
    int CheckInId,
    int TicketId,
    int? CounterId,
    int? EmployeeId,
    string Channel) : IRequest<CheckInEntity?>;

internal sealed class UpdateCheckInHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateCheckInCommand, CheckInEntity?>
{
    public async Task<CheckInEntity?> Handle(UpdateCheckInCommand request, CancellationToken ct)
    {
        var checkIn = await uow.Repository<CheckInEntity>().GetByIdAsync(request.CheckInId, ct);
        if (checkIn is null) return null;

        checkIn.TicketId = request.TicketId;
        checkIn.CounterId = request.CounterId;
        checkIn.EmployeeId = request.EmployeeId;
        checkIn.Channel = request.Channel;

        uow.Repository<CheckInEntity>().Update(checkIn);
        await uow.SaveChangesAsync(ct);

        return checkIn;
    }
}