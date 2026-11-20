using MediatR;
using Sky.Domain.Ports;
using PassengerEntity = Sky.Domain.Entities.Passenger;

namespace Sky.Application.UseCases.Passenger.Queries;

public sealed record GetPassengerByIdQuery(
    int PassengerId) : IRequest<PassengerEntity?>;

internal sealed class GetPassengerByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetPassengerByIdQuery, PassengerEntity?>
{
    public Task<PassengerEntity?> Handle(GetPassengerByIdQuery request, CancellationToken ct)
        => uow.Repository<PassengerEntity>().GetByIdAsync(request.PassengerId, ct);
}
