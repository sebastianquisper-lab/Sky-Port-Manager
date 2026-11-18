namespace Sky.Application.UseCases.Boarding.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingEntity = Sky.Domain.Entities.Boarding;


public sealed record GetBoardingsQuery : IRequest<List<BoardingEntity>>;

internal sealed class GetBoardingsHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingsQuery, List<BoardingEntity>>
{
    public Task<List<BoardingEntity>> Handle(GetBoardingsQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingEntity>().GetAllAsync(ct);
    }
}