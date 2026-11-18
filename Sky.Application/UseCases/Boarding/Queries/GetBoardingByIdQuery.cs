namespace Sky.Application.UseCases.Boarding.Queries;

using MediatR;
using Sky.Domain.Ports;
using BoardingEntity = Sky.Domain.Entities.Boarding;


public sealed record GetBoardingByIdQuery(int BoardingId) : IRequest<BoardingEntity?>;

internal sealed class GetBoardingByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBoardingByIdQuery, BoardingEntity?>
{
    public Task<BoardingEntity?> Handle(GetBoardingByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BoardingEntity>().GetByIdAsync(request.BoardingId, ct);
    }
}