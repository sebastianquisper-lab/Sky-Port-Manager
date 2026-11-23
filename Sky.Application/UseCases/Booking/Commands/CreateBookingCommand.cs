using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using BookingEntity = Sky.Domain.Entities.Booking;

namespace Sky.Application.UseCases.Booking.Commands;

/// <summary>El código de reserva (PNR) ya no lo envía el cliente: lo genera ICodeGenerator.</summary>
public sealed record CreateBookingCommand(
    int BookingStatusId,
    DateTime? ExpirationDateUtc,
    string? ContactEmail,
    string? ContactPhone) : IRequest<BookingEntity>;

internal sealed class CreateBookingHandler(IUnitOfWork uow, IDateTimeProvider clock, ICodeGenerator codes)
    : IRequestHandler<CreateBookingCommand, BookingEntity>
{
    public async Task<BookingEntity> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var booking = new BookingEntity
        {
            BookingStatusId = request.BookingStatusId,
            BookingCode = codes.GenerateBookingCode(),
            BookingDateUtc = clock.UtcNow,
            ExpirationDateUtc = request.ExpirationDateUtc,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone
        };

        await uow.Repository<BookingEntity>().AddAsync(booking, ct);
        await uow.SaveChangesAsync(ct);

        return booking;
    }
}