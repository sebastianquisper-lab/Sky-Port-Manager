namespace Sky.Application.UseCases.BoardingGate.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingGateEntity = Sky.Domain.Entities.BoardingGate;


public sealed record CreateBoardingGateCommand(
    int TerminalId,
    string GateCode,
    string GateType) : IRequest<BoardingGateEntity>;

internal sealed class CreateBoardingGateHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBoardingGateCommand, BoardingGateEntity>
{
    public async Task<BoardingGateEntity> Handle(CreateBoardingGateCommand request, CancellationToken ct)
    {
        var boardingGate = new BoardingGateEntity
        {
            TerminalId = request.TerminalId,
            GateCode = request.GateCode,
            GateType = request.GateType,
            IsActive = true
        };

        await uow.Repository<BoardingGateEntity>().AddAsync(boardingGate, ct);
        await uow.SaveChangesAsync(ct);

        return boardingGate;
    }
}