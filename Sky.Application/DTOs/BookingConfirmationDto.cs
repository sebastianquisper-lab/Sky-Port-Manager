using BookingEntity = Sky.Domain.Entities.Booking;
using PaymentEntity = Sky.Domain.Entities.Payment;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.DTOs;

/// <summary>Resultado compuesto de una reserva completa: reserva + boletos + pago.</summary>
public sealed record BookingConfirmationDto(
    BookingEntity Booking,
    IReadOnlyList<TicketEntity> Tickets,
    PaymentEntity Payment);
