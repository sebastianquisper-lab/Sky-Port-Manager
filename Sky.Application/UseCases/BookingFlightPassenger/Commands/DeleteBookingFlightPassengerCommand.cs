namespace Sky.Application.UseCases.BookingFlightPassenger.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingFlightPassengerEntity = Sky.Domain.Entities.BookingFlightPassenger;


public sealed record DeleteBookingFlightPassengerCommand(
    int BookingId,
    int FlightId,
    int PassengerId) : IRequest<bool>;

internal sealed class DeleteBookingFlightPassengerHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteBookingFlightPassengerCommand, bool>
{
    public async Task<bool> Handle(DeleteBookingFlightPassengerCommand request, CancellationToken ct)
    {
        var bookingFlightPassenger = await uow.Repository<BookingFlightPassengerEntity>().FirstOrDefaultAsync(x => x.BookingId == request.BookingId && x.FlightId == request.FlightId && x.PassengerId == request.PassengerId, ct);
        if (bookingFlightPassenger is null) return false;

        uow.Repository<BookingFlightPassengerEntity>().Delete(bookingFlightPassenger);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}