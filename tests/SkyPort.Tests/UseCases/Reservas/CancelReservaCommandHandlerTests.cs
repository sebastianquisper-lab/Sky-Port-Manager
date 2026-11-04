using NSubstitute;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Reservas;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.UseCases.Reservas;

public sealed class CancelReservaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTicketHasPassedCheckIn_RejectsWithoutSaving()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var reservations = Substitute.For<IGenericRepository<Reserva>>();
        var tickets = Substitute.For<IGenericRepository<Boleto>>();
        var reservation = new Reserva { ReservaId = 12, Estado = EstadoReserva.Confirmada };
        var ticket = new Boleto { BoletoId = 34, ReservaId = 12, Estado = EstadoBoleto.CheckInRealizado };
        unitOfWork.Reservas.Returns(reservations);
        unitOfWork.Boletos.Returns(tickets);
        reservations.GetByIdAsync(12, Arg.Any<CancellationToken>()).Returns(reservation);
        tickets.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Boleto, bool>>>(), Arg.Any<CancellationToken>()).Returns(ticket);
        tickets.GetByIdAsync(34, Arg.Any<CancellationToken>()).Returns(ticket);

        var result = await new CancelReservaCommandHandler(unitOfWork)
            .Handle(new CancelReservaCommand(12), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(EstadoReserva.Confirmada, reservation.Estado);
        await unitOfWork.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTicketIsIssued_ChangesTrackedEntitiesAndCompletesOnce()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var reservations = Substitute.For<IGenericRepository<Reserva>>();
        var tickets = Substitute.For<IGenericRepository<Boleto>>();
        var reservation = new Reserva { ReservaId = 12, Estado = EstadoReserva.Confirmada };
        var ticket = new Boleto { BoletoId = 34, ReservaId = 12, Estado = EstadoBoleto.Emitido };
        unitOfWork.Reservas.Returns(reservations);
        unitOfWork.Boletos.Returns(tickets);
        reservations.GetByIdAsync(12, Arg.Any<CancellationToken>()).Returns(reservation);
        tickets.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Boleto, bool>>>(), Arg.Any<CancellationToken>()).Returns(ticket);
        tickets.GetByIdAsync(34, Arg.Any<CancellationToken>()).Returns(ticket);

        var result = await new CancelReservaCommandHandler(unitOfWork)
            .Handle(new CancelReservaCommand(12), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(EstadoReserva.Cancelada, reservation.Estado);
        Assert.Equal(EstadoBoleto.Cancelado, ticket.Estado);
        await unitOfWork.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
        tickets.DidNotReceive().Update(Arg.Any<Boleto>());
        reservations.DidNotReceive().Update(Arg.Any<Reserva>());
    }
}
