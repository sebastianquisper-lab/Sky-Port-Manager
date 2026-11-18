namespace Sky.Application.UseCases.BoardingGate.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingGateEntity = Sky.Domain.Entities.BoardingGate;


public sealed record UpdateBoardingGateCommand(
    int GateId,
    int TerminalId,
    string GateCode,
    string GateType,
    bool IsActive) : IRequest<BoardingGateEntity?>;

internal sealed class UpdateBoardingGateHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBoardingGateCommand, BoardingGateEntity?>
{
    public async Task<BoardingGateEntity?> Handle(UpdateBoardingGateCommand request, CancellationToken ct)
    {
        var boardingGate = await uow.Repository<BoardingGateEntity>().GetByIdAsync(request.GateId, ct);
        if (boardingGate is null) return null;

        boardingGate.TerminalId = request.TerminalId;
        boardingGate.GateCode = request.GateCode;
        boardingGate.GateType = request.GateType;
        boardingGate.IsActive = request.IsActive;

        uow.Repository<BoardingGateEntity>().Update(boardingGate);
        await uow.SaveChangesAsync(ct);

        return boardingGate;
    }
}