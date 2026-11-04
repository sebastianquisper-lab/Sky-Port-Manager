using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Vuelos.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Vuelos;

public sealed class CreateVueloCommandTests
{
    private static readonly DateTime Departure = new(2027, 2, 3, 8, 0, 0, DateTimeKind.Unspecified);
    private static readonly DateTime Arrival = new(2027, 2, 3, 10, 0, 0, DateTimeKind.Unspecified);

    [Fact]
    public async Task Handle_CreatesProgrammedFlightWithoutGateAndCompletesOnce()
    {
        var (work, routes, aircraft, _, flights) = CreateWork();
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);
        var handler = new CreateVueloCommandHandler(work);

        var result = await handler.Handle(new(" SP101 ", 2, 3, null, Departure, Arrival), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await flights.Received(1).AddAsync(Arg.Is<Vuelo>(flight =>
            flight.NumeroVuelo == "SP101" && flight.RutaId == 2 && flight.AeronaveId == 3 &&
            flight.PuertaId == null && flight.FechaSalida == Departure &&
            flight.FechaSalida.Kind == DateTimeKind.Unspecified && flight.Estado == EstadoVuelo.Programado),
            Arg.Any<CancellationToken>());
        await routes.Received(1).GetByIdAsync(2, Arg.Any<CancellationToken>());
        await aircraft.Received(1).GetByIdAsync(3, Arg.Any<CancellationToken>());
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectsArrivalNotAfterDepartureWithoutSaving()
    {
        var (work, _, _, _, _) = CreateWork();
        var result = await new CreateVueloCommandHandler(work).Handle(
            new("SP101", 2, 3, null, Arrival, Departure), CancellationToken.None);

        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectsMissingGateReference()
    {
        var (work, _, _, gates, _) = CreateWork();
        gates.GetByIdAsync(9, Arg.Any<CancellationToken>()).Returns((Puerta?)null);

        var result = await new CreateVueloCommandHandler(work).Handle(
            new("SP101", 2, 3, 9, Departure, Arrival), CancellationToken.None);

        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectsExistingNumberAndDepartureCombination()
    {
        var (work, _, _, _, flights) = CreateWork();
        flights.FirstOrDefaultAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Vuelo());

        var result = await new CreateVueloCommandHandler(work).Handle(
            new("SP101", 2, 3, null, Departure, Arrival), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapsDuplicateKeyAtSaveToConflict()
    {
        var (work, _, _, _, _) = CreateWork();
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new DuplicateKeyException("duplicate"));

        var result = await new CreateVueloCommandHandler(work).Handle(
            new("SP101", 2, 3, null, Departure, Arrival), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
    }

    private static (
        IUnitOfWork Work,
        IGenericRepository<Ruta> Routes,
        IGenericRepository<Aeronave> Aircraft,
        IGenericRepository<Puerta> Gates,
        IGenericRepository<Vuelo> Flights) CreateWork()
    {
        var work = Substitute.For<IUnitOfWork>();
        var routes = Substitute.For<IGenericRepository<Ruta>>();
        var aircraft = Substitute.For<IGenericRepository<Aeronave>>();
        var gates = Substitute.For<IGenericRepository<Puerta>>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        work.Rutas.Returns(routes);
        work.Aeronaves.Returns(aircraft);
        work.Puertas.Returns(gates);
        work.Vuelos.Returns(flights);
        routes.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(new Ruta { RutaId = 2 });
        aircraft.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(new Aeronave { AeronaveId = 3 });
        flights.FirstOrDefaultAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Vuelo?)null);
        return (work, routes, aircraft, gates, flights);
    }
}
