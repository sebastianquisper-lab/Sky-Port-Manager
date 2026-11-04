using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Puertas.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Puertas;

public sealed class DeletePuertaTests
{
    [Fact]
    public async Task DeleteRejectsFlightAndBoardingReferences()
    {
        foreach (var (flightCount, boardingCount) in new[] { (1, 0), (0, 1) })
        {
            var work = Substitute.For<IUnitOfWork>();
            var gates = Substitute.For<IGenericRepository<Puerta>>();
            var flights = Substitute.For<IGenericRepository<Vuelo>>();
            var boardings = Substitute.For<IGenericRepository<Embarque>>();
            work.Puertas.Returns(gates);
            work.Vuelos.Returns(flights);
            work.Embarques.Returns(boardings);
            gates.GetByIdAsync(6, Arg.Any<CancellationToken>()).Returns(new Puerta { PuertaId = 6 });
            flights.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(flightCount);
            boardings.CountAsync(Arg.Any<Expression<Func<Embarque, bool>>>(), Arg.Any<CancellationToken>()).Returns(boardingCount);

            var result = await new DeletePuertaCommandHandler(work).Handle(new(6), CancellationToken.None);

            Assert.Equal(ErrorType.Conflict, result.Error?.Type);
            gates.DidNotReceive().Remove(Arg.Any<Puerta>());
            await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
        }
    }

    [Fact]
    public async Task DeleteRemovesUnreferencedGateAndTreatsZeroChangesAsSuccess()
    {
        var work = Substitute.For<IUnitOfWork>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        work.Puertas.Returns(gates);
        work.Vuelos.Returns(Substitute.For<IGenericRepository<Vuelo>>());
        work.Embarques.Returns(Substitute.For<IGenericRepository<Embarque>>());
        var gate = new Puerta { PuertaId = 6 };
        gates.GetByIdAsync(6, Arg.Any<CancellationToken>()).Returns(gate);
        work.Vuelos.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.Embarques.CountAsync(Arg.Any<Expression<Func<Embarque, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new DeletePuertaCommandHandler(work).Handle(new(6), CancellationToken.None);

        Assert.True(result.IsSuccess);
        gates.Received(1).Remove(gate);
    }

    [Fact]
    public async Task DeleteMapsForeignKeyRaceToConflict()
    {
        var work = Substitute.For<IUnitOfWork>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        work.Puertas.Returns(gates);
        work.Vuelos.Returns(Substitute.For<IGenericRepository<Vuelo>>());
        work.Embarques.Returns(Substitute.For<IGenericRepository<Embarque>>());
        gates.GetByIdAsync(6, Arg.Any<CancellationToken>()).Returns(new Puerta { PuertaId = 6 });
        work.Vuelos.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.Embarques.CountAsync(Arg.Any<Expression<Func<Embarque, bool>>>(), Arg.Any<CancellationToken>()).Returns(0);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new ReferenceConflictException(new Exception()));

        var result = await new DeletePuertaCommandHandler(work).Handle(new(6), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
    }
}
