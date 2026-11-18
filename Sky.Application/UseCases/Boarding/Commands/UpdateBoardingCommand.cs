namespace Sky.Application.UseCases.Boarding.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingEntity = Sky.Domain.Entities.Boarding;


public sealed record UpdateBoardingCommand(
    int BoardingId,
    int TicketId,
    int GateId,
    int? EmployeeId) : IRequest<BoardingEntity?>;

internal sealed class UpdateBoardingHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBoardingCommand, BoardingEntity?>
{
    public async Task<BoardingEntity?> Handle(UpdateBoardingCommand request, CancellationToken ct)
    {
        var boarding = await uow.Repository<BoardingEntity>().GetByIdAsync(request.BoardingId, ct);
        if (boarding is null) return null;

        boarding.TicketId = request.TicketId;
        boarding.GateId = request.GateId;
        boarding.EmployeeId = request.EmployeeId;

        uow.Repository<BoardingEntity>().Update(boarding);
        await uow.SaveChangesAsync(ct);

        return boarding;
    }
}