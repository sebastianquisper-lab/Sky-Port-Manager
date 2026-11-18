namespace Sky.Application.UseCases.BoardingPass.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingPassEntity = Sky.Domain.Entities.BoardingPass;


public sealed record CreateBoardingPassCommand(
    int TicketId,
    string? BoardingGroup,
    short? SequenceNumber,
    string Barcode) : IRequest<BoardingPassEntity>;

internal sealed class CreateBoardingPassHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBoardingPassCommand, BoardingPassEntity>
{
    public async Task<BoardingPassEntity> Handle(CreateBoardingPassCommand request, CancellationToken ct)
    {
        var boardingPass = new BoardingPassEntity
        {
            TicketId = request.TicketId,
            BoardingGroup = request.BoardingGroup,
            SequenceNumber = request.SequenceNumber,
            Barcode = request.Barcode,
            IssuedAtUtc = DateTime.UtcNow
        };

        await uow.Repository<BoardingPassEntity>().AddAsync(boardingPass, ct);
        await uow.SaveChangesAsync(ct);

        return boardingPass;
    }
}