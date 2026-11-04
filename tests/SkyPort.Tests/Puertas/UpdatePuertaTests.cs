using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Puertas.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Puertas;

public sealed class UpdatePuertaTests
{
    [Fact]
    public async Task AvailabilityCanChangeEvenWhenGateHasHistory()
    {
        var work = Substitute.For<IUnitOfWork>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        var terminals = Substitute.For<IGenericRepository<Terminal>>();
        work.Puertas.Returns(gates);
        work.Terminales.Returns(terminals);
        var gate = new Puerta { PuertaId = 4, TerminalId = 1, Codigo = "A1", Disponible = true };
        gates.GetByIdAsync(4, Arg.Any<CancellationToken>()).Returns(gate);
        terminals.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 1, Nombre = "Norte" });
        gates.FirstOrDefaultAsync(Arg.Any<Expression<Func<Puerta, bool>>>(), Arg.Any<CancellationToken>()).Returns((Puerta?)null);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new UpdatePuertaCommandHandler(work).Handle(new(4, 1, "A1", false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(gate.Disponible);
        await work.DidNotReceive().Vuelos.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>());
        await work.DidNotReceive().Embarques.CountAsync(Arg.Any<Expression<Func<Embarque, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AssignmentChangeIsBlockedWhenFlightReferencesGate()
    {
        var work = Substitute.For<IUnitOfWork>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        var terminals = Substitute.For<IGenericRepository<Terminal>>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        work.Puertas.Returns(gates);
        work.Terminales.Returns(terminals);
        work.Vuelos.Returns(flights);
        var gate = new Puerta { PuertaId = 4, TerminalId = 1, Codigo = "A1", Disponible = true };
        gates.GetByIdAsync(4, Arg.Any<CancellationToken>()).Returns(gate);
        terminals.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 1, Nombre = "Norte" });
        flights.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(1);

        var result = await new UpdatePuertaCommandHandler(work).Handle(new(4, 1, "A2", false), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        Assert.Equal("A1", gate.Codigo);
        Assert.True(gate.Disponible);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AssignmentChangeIsAllowedWhenGateIsUnreferenced()
    {
        var work = Substitute.For<IUnitOfWork>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        var terminals = Substitute.For<IGenericRepository<Terminal>>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        var boardings = Substitute.For<IGenericRepository<Embarque>>();
        work.Puertas.Returns(gates);
        work.Terminales.Returns(terminals);
        work.Vuelos.Returns(flights);
        work.Embarques.Returns(boardings);
        var gate = new Puerta { PuertaId = 4, TerminalId = 1, Codigo = "A1", Disponible = true };
        gates.GetByIdAsync(4, Arg.Any<CancellationToken>()).Returns(gate);
        terminals.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(new Terminal { TerminalId = 2, Nombre = "Sur" });
        flights.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        boardings.CountAsync(Arg.Any<Expression<Func<Embarque, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        gates.FirstOrDefaultAsync(Arg.Any<Expression<Func<Puerta, bool>>>(), Arg.Any<CancellationToken>()).Returns((Puerta?)null);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new UpdatePuertaCommandHandler(work).Handle(new(4, 2, "B2", true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, gate.TerminalId);
        Assert.Equal("B2", gate.Codigo);
    }
}
