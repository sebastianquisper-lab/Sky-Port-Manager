namespace Sky.Application.UseCases.BoardingGate.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingGateEntity = Sky.Domain.Entities.BoardingGate;


public sealed record GetBoardingGatesQuery : IRequest<IReadOnlyList<BoardingGateEntity>>;

internal sealed class GetBoardingGatesHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingGatesQuery, IReadOnlyList<BoardingGateEntity>>
{
    public Task<IReadOnlyList<BoardingGateEntity>> Handle(GetBoardingGatesQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingGateEntity>().GetAllAsync(ct);
    }
}