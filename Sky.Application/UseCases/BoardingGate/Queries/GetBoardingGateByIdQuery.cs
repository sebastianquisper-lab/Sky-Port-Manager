namespace Sky.Application.UseCases.BoardingGate.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingGateEntity = Sky.Domain.Entities.BoardingGate;


public sealed record GetBoardingGateByIdQuery(int GateId) : IRequest<BoardingGateEntity?>;

internal sealed class GetBoardingGateByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingGateByIdQuery, BoardingGateEntity?>
{
    public Task<BoardingGateEntity?> Handle(GetBoardingGateByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingGateEntity>().GetByIdAsync(request.GateId, ct);
    }
}