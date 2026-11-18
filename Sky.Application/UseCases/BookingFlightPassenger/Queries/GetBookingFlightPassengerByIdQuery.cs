namespace Sky.Application.UseCases.BookingFlightPassenger.Queries;

using MediatR;
using Sky.Domain.Ports;
using BookingFlightPassengerEntity = Sky.Domain.Entities.BookingFlightPassenger;


public sealed record GetBookingFlightPassengerByIdQuery(
    int BookingId,
    int FlightId,
    int PassengerId) : IRequest<BookingFlightPassengerEntity?>;

internal sealed class GetBookingFlightPassengerByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetBookingFlightPassengerByIdQuery, BookingFlightPassengerEntity?>
{
    public Task<BookingFlightPassengerEntity?> Handle(GetBookingFlightPassengerByIdQuery request, CancellationToken ct)
    {
        return uow.Repository<BookingFlightPassengerEntity>().FirstOrDefaultAsync(x => x.BookingId == request.BookingId && x.FlightId == request.FlightId && x.PassengerId == request.PassengerId, ct);
    }
}