namespace Sky.Application.UseCases.BoardingPass.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingPassEntity = Sky.Domain.Entities.BoardingPass;


public sealed record GetBoardingPassByIdQuery(int BoardingPassId) : IRequest<BoardingPassEntity?>;

internal sealed class GetBoardingPassByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingPassByIdQuery, BoardingPassEntity?>
{
    public Task<BoardingPassEntity?> Handle(GetBoardingPassByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingPassEntity>().GetByIdAsync(request.BoardingPassId, ct);
    }
}