using MediatR;
using Sky.Domain.Ports;
using VFlightBoardEntity = Sky.Domain.Entities.VFlightBoard;

namespace Sky.Application.UseCases.VFlightBoard.Queries;

public sealed record GetFlightBoardQuery : IRequest<IReadOnlyList<VFlightBoardEntity>>;

internal sealed class GetFlightBoardHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightBoardQuery, IReadOnlyList<VFlightBoardEntity>>
{
    public Task<IReadOnlyList<VFlightBoardEntity>> Handle(GetFlightBoardQuery request, CancellationToken ct)
        => uow.Repository<VFlightBoardEntity>().GetAllAsync(ct);
}
