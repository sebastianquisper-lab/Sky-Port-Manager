namespace Sky.Application.UseCases.BoardingPass.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingPassEntity = Sky.Domain.Entities.BoardingPass;


public sealed record GetBoardingPassesQuery : IRequest<List<BoardingPassEntity>>;

internal sealed class GetBoardingPassesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingPassesQuery, List<BoardingPassEntity>>
{
    public Task<List<BoardingPassEntity>> Handle(GetBoardingPassesQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingPassEntity>().GetAllAsync(ct);
    }
}