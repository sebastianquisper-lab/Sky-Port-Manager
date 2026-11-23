namespace Sky.Application.UseCases.BoardingPass.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingPassEntity = Sky.Domain.Entities.BoardingPass;


public sealed record GetBoardingPassesQuery : IRequest<IReadOnlyList<BoardingPassEntity>>;

internal sealed class GetBoardingPassesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingPassesQuery, IReadOnlyList<BoardingPassEntity>>
{
    public Task<IReadOnlyList<BoardingPassEntity>> Handle(GetBoardingPassesQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingPassEntity>().GetAllAsync(ct);
    }
}