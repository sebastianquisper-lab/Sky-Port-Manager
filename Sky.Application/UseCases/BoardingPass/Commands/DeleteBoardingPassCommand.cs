namespace Sky.Application.UseCases.BoardingPass.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingPassEntity = Sky.Domain.Entities.BoardingPass;


public sealed record DeleteBoardingPassCommand(int BoardingPassId) : IRequest<bool>;

internal sealed class DeleteBoardingPassHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBoardingPassCommand, bool>
{
    public async Task<bool> Handle(DeleteBoardingPassCommand request, CancellationToken ct)
    {
        var boardingPass = await uow.Repository<BoardingPassEntity>().GetByIdAsync(request.BoardingPassId, ct);
        if (boardingPass is null) return false;

        uow.Repository<BoardingPassEntity>().Delete(boardingPass);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}