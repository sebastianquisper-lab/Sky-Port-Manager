using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Embarques.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Embarques;

public sealed class CreateEmbarqueCommandTests
{
    [Fact]
    public async Task Handle_RegistersBoardingAndAbordaTicketInSingleCompletion()
    {
        var (work, ticket, _, _, _, boardings, _) = CreateWork(gateId: 6);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);
        var handler = new CreateEmbarqueCommandHandler(work);

        var result = await handler.Handle(new CreateEmbarqueCommand(ticket.BoletoId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(EstadoBoleto.Abordado, ticket.Estado);
        await boardings.Received(1).AddAsync(Arg.Is<Embarque>(boarding =>
            boarding.BoletoId == ticket.BoletoId && boarding.PuertaId == 6 && boarding.FechaHora != default),
            Arg.Any<CancellationToken>());
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PreservesNullableGateWhenFlightHasNone()
    {
        var (work, ticket, _, _, _, boardings, _) = CreateWork(gateId: null);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new CreateEmbarqueCommandHandler(work).Handle(new(ticket.BoletoId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await boardings.Received(1).AddAsync(Arg.Is<Embarque>(boarding => boarding.PuertaId == null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RequiresExistingCheckIn()
    {
        var (work, ticket, _, checkIns, _, _, _) = CreateWork();
        checkIns.FirstOrDefaultAsync(Arg.Any<Expression<Func<Checkin, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Checkin?)null);

        var result = await new CreateEmbarqueCommandHandler(work).Handle(new(ticket.BoletoId), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectsCancelledTicket()
    {
        var (work, ticket, _, _, _, _, _) = CreateWork();
        ticket.Estado = EstadoBoleto.Cancelado;

        var result = await new CreateEmbarqueCommandHandler(work).Handle(new(ticket.BoletoId), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectsUnconfirmedReservation()
    {
        var (work, ticket, reservation, _, _, _, _) = CreateWork();
        reservation.Estado = EstadoReserva.Cancelada;

        var result = await new CreateEmbarqueCommandHandler(work).Handle(new(ticket.BoletoId), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectsMissingAssignedGate()
    {
        var (work, ticket, _, _, gates, _, _) = CreateWork(gateId: 6);
        gates.GetByIdAsync(6, Arg.Any<CancellationToken>()).Returns((Puerta?)null);

        var result = await new CreateEmbarqueCommandHandler(work).Handle(new(ticket.BoletoId), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectsDuplicateBoarding()
    {
        var (work, ticket, _, _, _, boardings, _) = CreateWork();
        boardings.FirstOrDefaultAsync(Arg.Any<Expression<Func<Embarque, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Embarque { BoletoId = ticket.BoletoId });

        var result = await new CreateEmbarqueCommandHandler(work).Handle(new(ticket.BoletoId), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapsDuplicateKeyRaceToConflict()
    {
        var (work, ticket, _, _, _, _, _) = CreateWork();
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new DuplicateKeyException("duplicate"));

        var result = await new CreateEmbarqueCommandHandler(work).Handle(new(ticket.BoletoId), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
    }

    private static (
        IUnitOfWork Work,
        Boleto Ticket,
        Reserva Reservation,
        IGenericRepository<Checkin> CheckIns,
        IGenericRepository<Puerta> Gates,
        IGenericRepository<Embarque> Boardings,
        IGenericRepository<Vuelo> Flights) CreateWork(int? gateId = null)
    {
        var work = Substitute.For<IUnitOfWork>();
        var tickets = Substitute.For<IGenericRepository<Boleto>>();
        var reservations = Substitute.For<IGenericRepository<Reserva>>();
        var checkIns = Substitute.For<IGenericRepository<Checkin>>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        var boardings = Substitute.For<IGenericRepository<Embarque>>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        var ticket = new Boleto { BoletoId = 10, ReservaId = 20, Estado = EstadoBoleto.CheckInRealizado };
        var reservation = new Reserva { ReservaId = 20, VueloId = 30, Estado = EstadoReserva.Confirmada };

        work.Boletos.Returns(tickets);
        work.Reservas.Returns(reservations);
        work.CheckIns.Returns(checkIns);
        work.Puertas.Returns(gates);
        work.Embarques.Returns(boardings);
        work.Vuelos.Returns(flights);
        tickets.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(ticket);
        checkIns.FirstOrDefaultAsync(Arg.Any<Expression<Func<Checkin, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Checkin { BoletoId = 10 });
        reservations.GetByIdAsync(20, Arg.Any<CancellationToken>()).Returns(reservation);
        flights.GetByIdAsync(30, Arg.Any<CancellationToken>()).Returns(new Vuelo { VueloId = 30, PuertaId = gateId });
        boardings.FirstOrDefaultAsync(Arg.Any<Expression<Func<Embarque, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Embarque?)null);
        if (gateId.HasValue)
        {
            gates.GetByIdAsync(gateId.Value, Arg.Any<CancellationToken>()).Returns(new Puerta { PuertaId = gateId.Value });
        }

        return (work, ticket, reservation, checkIns, gates, boardings, flights);
    }
}
