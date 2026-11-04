using NSubstitute;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.CheckIn.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.UseCases.CheckIn;

public sealed class CreateCheckInCommandHandlerTests
{
    private static (IUnitOfWork Unit, IGenericRepository<Boleto> Tickets, IGenericRepository<Reserva> Reservations,
        IGenericRepository<Checkin> CheckIns, IGenericRepository<Empleado> Employees) Setup(
        string ticketState = EstadoBoleto.Emitido, string reservationState = EstadoReserva.Confirmada)
    {
        var unit = Substitute.For<IUnitOfWork>();
        var tickets = Substitute.For<IGenericRepository<Boleto>>();
        var reservations = Substitute.For<IGenericRepository<Reserva>>();
        var checkIns = Substitute.For<IGenericRepository<Checkin>>();
        var employees = Substitute.For<IGenericRepository<Empleado>>();
        unit.Boletos.Returns(tickets); unit.Reservas.Returns(reservations); unit.CheckIns.Returns(checkIns); unit.Empleados.Returns(employees);
        tickets.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(new Boleto { BoletoId = 3, ReservaId = 4, Estado = ticketState });
        reservations.GetByIdAsync(4, Arg.Any<CancellationToken>()).Returns(new Reserva { ReservaId = 4, Estado = reservationState });
        checkIns.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Checkin, bool>>>(), Arg.Any<CancellationToken>()).Returns((Checkin?)null);
        employees.GetByIdAsync(8, Arg.Any<CancellationToken>()).Returns(new Empleado { EmpleadoId = 8 });
        return (unit, tickets, reservations, checkIns, employees);
    }

    [Fact]
    public async Task Handle_ValidCheckInUpdatesTicketAndAddsRecordBeforeSingleCompletion()
    {
        var (unit, tickets, _, checkIns, _) = Setup();
        var ticket = await tickets.GetByIdAsync(3);
        Checkin? added = null;
        checkIns.AddAsync(Arg.Do<Checkin>(x => added = x), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        unit.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new CreateCheckInCommandHandler(unit).Handle(new CreateCheckInCommand(3, null, true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(EstadoBoleto.CheckInRealizado, ticket!.Estado);
        Assert.NotNull(added);
        Assert.True(added!.DocumentoVerificado);
        Assert.Null(added.EmpleadoId);
        await unit.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnverifiedDocumentIsRejected()
    {
        var (unit, _, _, _, _) = Setup();
        var result = await new CreateCheckInCommandHandler(unit).Handle(new CreateCheckInCommand(3, null, false), CancellationToken.None);
        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("Cancelado", "Confirmada")]
    [InlineData("Emitido", "Cancelada")]
    public async Task Handle_CancelledTicketOrReservationIsRejected(string ticketState, string reservationState)
    {
        var (unit, _, _, _, _) = Setup(ticketState, reservationState);
        var result = await new CreateCheckInCommandHandler(unit).Handle(new CreateCheckInCommand(3, null, true), CancellationToken.None);
        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MissingTicketIsRejected()
    {
        var (unit, tickets, _, _, _) = Setup();
        tickets.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns((Boleto?)null);
        var result = await new CreateCheckInCommandHandler(unit).Handle(new CreateCheckInCommand(3, null, true), CancellationToken.None);
        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateCheckInIsRejected()
    {
        var (unit, _, _, checkIns, _) = Setup();
        checkIns.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Checkin, bool>>>(), Arg.Any<CancellationToken>()).Returns(new Checkin { BoletoId = 3 });
        var result = await new CreateCheckInCommandHandler(unit).Handle(new CreateCheckInCommand(3, null, true), CancellationToken.None);
        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MissingSpecifiedEmployeeIsRejected()
    {
        var (unit, _, _, _, employees) = Setup();
        employees.GetByIdAsync(8, Arg.Any<CancellationToken>()).Returns((Empleado?)null);
        var result = await new CreateCheckInCommandHandler(unit).Handle(new CreateCheckInCommand(3, 8, true), CancellationToken.None);
        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
}
