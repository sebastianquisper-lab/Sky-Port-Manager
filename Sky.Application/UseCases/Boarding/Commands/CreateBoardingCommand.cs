namespace Sky.Application.UseCases.Boarding.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingEntity = Sky.Domain.Entities.Boarding;


public sealed record CreateBoardingCommand(
    int TicketId,
    int GateId,
    int? EmployeeId) : IRequest<BoardingEntity>;

internal sealed class CreateBoardingHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBoardingCommand, BoardingEntity>
{
    public async Task<BoardingEntity> Handle(CreateBoardingCommand request, CancellationToken ct)
    {
        var boarding = new BoardingEntity
        {
            TicketId = request.TicketId,
            GateId = request.GateId,
            EmployeeId = request.EmployeeId,
            BoardedAtUtc = DateTime.UtcNow
        };

        await uow.Repository<BoardingEntity>().AddAsync(boarding, ct);
        await uow.SaveChangesAsync(ct);

        return boarding;
    }
}