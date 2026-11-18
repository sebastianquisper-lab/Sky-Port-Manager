namespace Sky.Application.UseCases.BoardingPass.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingPassEntity = Sky.Domain.Entities.BoardingPass;


public sealed record UpdateBoardingPassCommand(
    int BoardingPassId,
    int TicketId,
    string? BoardingGroup,
    short? SequenceNumber,
    string Barcode) : IRequest<BoardingPassEntity?>;

internal sealed class UpdateBoardingPassHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBoardingPassCommand, BoardingPassEntity?>
{
    public async Task<BoardingPassEntity?> Handle(UpdateBoardingPassCommand request, CancellationToken ct)
    {
        var boardingPass = await uow.Repository<BoardingPassEntity>().GetByIdAsync(request.BoardingPassId, ct);
        if (boardingPass is null) return null;

        boardingPass.TicketId = request.TicketId;
        boardingPass.BoardingGroup = request.BoardingGroup;
        boardingPass.SequenceNumber = request.SequenceNumber;
        boardingPass.Barcode = request.Barcode;

        uow.Repository<BoardingPassEntity>().Update(boardingPass);
        await uow.SaveChangesAsync(ct);

        return boardingPass;
    }
}