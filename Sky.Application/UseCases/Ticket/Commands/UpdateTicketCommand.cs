using MediatR;
using Sky.Domain.Ports;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.Ticket.Commands;

public sealed record UpdateTicketCommand(
    int TicketId,
    int BookingId,
    int FlightId,
    int PassengerId,
    int TicketStatusId,
    int CabinClassId,
    int? FareId,
    string? SeatNumber,
    decimal Price,
    decimal TaxAmount,
    string CurrencyCode) : IRequest<TicketEntity?>;

internal sealed class UpdateTicketHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateTicketCommand, TicketEntity?>
{
    public async Task<TicketEntity?> Handle(UpdateTicketCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<TicketEntity>().GetByIdAsync(request.TicketId, ct);
        if (entity is null) return null;

        entity.BookingId = request.BookingId;
        entity.FlightId = request.FlightId;
        entity.PassengerId = request.PassengerId;
        entity.TicketStatusId = request.TicketStatusId;
        entity.CabinClassId = request.CabinClassId;
        entity.FareId = request.FareId;
        entity.SeatNumber = request.SeatNumber;
        entity.Price = request.Price;
        entity.TaxAmount = request.TaxAmount;
        entity.CurrencyCode = request.CurrencyCode;

        uow.Repository<TicketEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
