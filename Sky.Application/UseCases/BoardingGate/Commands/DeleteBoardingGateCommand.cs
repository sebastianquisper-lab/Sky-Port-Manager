namespace Sky.Application.UseCases.BoardingGate.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingGateEntity = Sky.Domain.Entities.BoardingGate;


public sealed record DeleteBoardingGateCommand(int GateId) : IRequest<bool>;

internal sealed class DeleteBoardingGateHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBoardingGateCommand, bool>
{
    public async Task<bool> Handle(DeleteBoardingGateCommand request, CancellationToken ct)
    {
        var boardingGate = await uow.Repository<BoardingGateEntity>().GetByIdAsync(request.GateId, ct);
        if (boardingGate is null) return false;

        uow.Repository<BoardingGateEntity>().Delete(boardingGate);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}