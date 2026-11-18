namespace Sky.Application.UseCases.Boarding.Commands;

using MediatR;
using Sky.Domain.Ports;
using BoardingEntity = Sky.Domain.Entities.Boarding;


public sealed record DeleteBoardingCommand(int BoardingId) : IRequest<bool>;

internal sealed class DeleteBoardingHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBoardingCommand, bool>
{
    public async Task<bool> Handle(DeleteBoardingCommand request, CancellationToken ct)
    {
        var boarding = await uow.Repository<BoardingEntity>().GetByIdAsync(request.BoardingId, ct);
        if (boarding is null) return false;

        uow.Repository<BoardingEntity>().Delete(boarding);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}