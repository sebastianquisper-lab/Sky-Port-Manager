namespace Sky.Application.UseCases.CheckIn.Queries;

using MediatR;
using Sky.Domain.Ports;
using CheckInEntity = Sky.Domain.Entities.CheckIn;


public sealed record GetCheckInsQuery : IRequest<List<CheckInEntity>>;

internal sealed class GetCheckInsHandler(IUnitOfWork uow)
    : IRequestHandler<GetCheckInsQuery, List<CheckInEntity>>
{
    public Task<List<CheckInEntity>> Handle(GetCheckInsQuery request, CancellationToken ct)
    {
        return uow.Repository<CheckInEntity>().GetAllAsync(ct);
    }
}