namespace Sky.Application.UseCases.CheckInCounter.Queries;

using MediatR;
using Sky.Domain.Ports;
using CheckInCounterEntity = Sky.Domain.Entities.CheckInCounter;


public sealed record GetCheckInCounterByIdQuery(int CounterId) : IRequest<CheckInCounterEntity?>;

internal sealed class GetCheckInCounterByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetCheckInCounterByIdQuery, CheckInCounterEntity?>
{
    public Task<CheckInCounterEntity?> Handle(GetCheckInCounterByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<CheckInCounterEntity>().GetByIdAsync(request.CounterId, ct);
    }
}