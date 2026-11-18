namespace Sky.Application.UseCases.Booking.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingEntity = Sky.Domain.Entities.Booking;


public sealed record UpdateBookingCommand(
    int BookingId,
    int BookingStatusId,
    string BookingCode,
    DateTime? ExpirationDateUtc,
    string? ContactEmail,
    string? ContactPhone) : IRequest<BookingEntity?>;

internal sealed class UpdateBookingHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateBookingCommand, BookingEntity?>
{
    public async Task<BookingEntity?> Handle(UpdateBookingCommand request, CancellationToken ct)
    {
        var booking = await uow.Repository<BookingEntity>().GetByIdAsync(request.BookingId, ct);
        if (booking is null) return null;

        booking.BookingStatusId = request.BookingStatusId;
        booking.BookingCode = request.BookingCode;
        booking.ExpirationDateUtc = request.ExpirationDateUtc;
        booking.ContactEmail = request.ContactEmail;
        booking.ContactPhone = request.ContactPhone;

        uow.Repository<BookingEntity>().Update(booking);
        await uow.SaveChangesAsync(ct);

        return booking;
    }
}