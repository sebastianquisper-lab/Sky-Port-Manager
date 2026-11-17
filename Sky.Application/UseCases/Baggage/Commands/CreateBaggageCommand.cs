using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;
using BaggageEntity = Sky.Domain.Entities.Baggage;
using BaggageTypeEntity = Sky.Domain.Entities.BaggageType;
using FlightEntity = Sky.Domain.Entities.Flight;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.Baggage.Commands;

public sealed record CreateBaggageCommand(
    int TicketId,
    int BaggageTypeId,
    int BaggageStatusId,
    decimal WeightKg,
    string? Description) : IRequest<BaggageEntity>;

internal sealed class CreateBaggageHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICodeGenerator codes)
    : IRequestHandler<CreateBaggageCommand, BaggageEntity>
{
    public async Task<BaggageEntity> Handle(CreateBaggageCommand request, CancellationToken ct)
    {
        var ticket = await uow.Repository<TicketEntity>().GetByIdAsync(request.TicketId, ct)
            ?? throw new KeyNotFoundException($"El boleto {request.TicketId} no existe.");

        var baggageType = await uow.Repository<BaggageTypeEntity>().GetByIdAsync(request.BaggageTypeId, ct)
            ?? throw new KeyNotFoundException($"El tipo de equipaje {request.BaggageTypeId} no existe.");

        // Regla que la BD no cubre: el peso no puede superar el máximo del tipo de equipaje.
        if (baggageType.MaxWeightKg is decimal maxWeight && request.WeightKg > maxWeight)
        {
            throw new InvalidOperationException(
                $"El equipaje pesa {request.WeightKg} kg y el máximo para '{baggageType.TypeName}' es {maxWeight} kg.");
        }

        // La etiqueta se genera con el código IATA de la aerolínea del vuelo del boleto.
        var flight = await uow.Repository<FlightEntity>().GetByIdAsync(ticket.FlightId, ct)
            ?? throw new KeyNotFoundException($"El vuelo {ticket.FlightId} no existe.");

        var airline = await uow.Repository<AirlineEntity>().GetByIdAsync(flight.AirlineId, ct)
            ?? throw new KeyNotFoundException($"La aerolínea {flight.AirlineId} no existe.");

        var entity = new BaggageEntity
        {
            TicketId = request.TicketId,
            BaggageTypeId = request.BaggageTypeId,
            BaggageStatusId = request.BaggageStatusId,
            TagNumber = codes.GenerateBaggageTagNumber(airline.IataCode),
            WeightKg = request.WeightKg,
            Description = request.Description,
            CreatedAtUtc = clock.UtcNow
        };

        await uow.Repository<BaggageEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
