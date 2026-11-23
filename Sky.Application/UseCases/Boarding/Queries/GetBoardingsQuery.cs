namespace Sky.Application.UseCases.Boarding.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingEntity = Sky.Domain.Entities.Boarding;


public sealed record GetBoardingsQuery : IRequest<IReadOnlyList<BoardingEntity>>;

internal sealed class GetBoardingsHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingsQuery, IReadOnlyList<BoardingEntity>>
{
    public Task<IReadOnlyList<BoardingEntity>> Handle(GetBoardingsQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingEntity>().GetAllAsync(ct);
    }
}