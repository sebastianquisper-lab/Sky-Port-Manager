namespace Sky.Application.UseCases.CheckIn.Commands;

using MediatR;
using Sky.Domain.Ports;
using CheckInEntity = Sky.Domain.Entities.CheckIn;


public sealed record DeleteCheckInCommand(int CheckInId) : IRequest<bool>;

internal sealed class DeleteCheckInHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteCheckInCommand, bool>
{
    public async Task<bool> Handle(DeleteCheckInCommand request, CancellationToken ct)
    {
        var checkIn = await uow.Repository<CheckInEntity>().GetByIdAsync(request.CheckInId, ct);
        if (checkIn is null) return false;

        uow.Repository<CheckInEntity>().Delete(checkIn);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}