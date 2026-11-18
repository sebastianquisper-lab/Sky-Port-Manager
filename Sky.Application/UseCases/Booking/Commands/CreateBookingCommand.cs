namespace Sky.Application.UseCases.Booking.Commands;

using MediatR;
using Sky.Domain.Ports;
using BookingEntity = Sky.Domain.Entities.Booking;


public sealed record CreateBookingCommand(
    int BookingStatusId,
    string BookingCode,
    DateTime? ExpirationDateUtc,
    string? ContactEmail,
    string? ContactPhone) : IRequest<BookingEntity>;

internal sealed class CreateBookingHandler(IUnitOfWork uow)
    : IRequestHandler<CreateBookingCommand, BookingEntity>
{
    public async Task<BookingEntity> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var booking = new BookingEntity
        {
            BookingStatusId = request.BookingStatusId,
            BookingCode = request.BookingCode,
            BookingDateUtc = DateTime.UtcNow,
            ExpirationDateUtc = request.ExpirationDateUtc,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone
        };

        await uow.Repository<BookingEntity>().AddAsync(booking, ct);
        await uow.SaveChangesAsync(ct);

        return booking;
    }
}