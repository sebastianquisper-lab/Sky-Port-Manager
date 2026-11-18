namespace Sky.Application.UseCases.BookingFlightPassenger.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingFlightPassengerEntity = Sky.Domain.Entities.BookingFlightPassenger;


public sealed record CreateBookingFlightPassengerCommand(
    int BookingId,
    int FlightId,
    int PassengerId) : IRequest<BookingFlightPassengerEntity>;

internal sealed class CreateBookingFlightPassengerHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBookingFlightPassengerCommand, BookingFlightPassengerEntity>
{
    public async Task<BookingFlightPassengerEntity> Handle(CreateBookingFlightPassengerCommand request, CancellationToken ct)
    {
        var bookingFlightPassenger = new BookingFlightPassengerEntity
        {
            BookingId = request.BookingId,
            FlightId = request.FlightId,
            PassengerId = request.PassengerId
        };

        await uow.Repository<BookingFlightPassengerEntity>().AddAsync(bookingFlightPassenger, ct);
        await uow.SaveChangesAsync(ct);

        return bookingFlightPassenger;
    }
}