using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.Rules;
using SkyPort.Application.Dtos.Reservas;
using SkyPort.Application.UseCases.Reservas.Queries;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Reservas.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.UseCases.Reservas;

public sealed class ReservaExistingQueryHandlerTests
{
    [Fact]
    public async Task GetOpenFlights_ReturnsPortResult()
    {
        var queries = Substitute.For<IReservaQueries>();
        var expected = (IReadOnlyList<VueloAbiertoDto>)[];
        queries.GetVuelosAbiertosAsync(Arg.Any<CancellationToken>()).Returns(expected);
        Assert.Same(expected, await new GetVuelosAbiertosQueryHandler(queries).Handle(new GetVuelosAbiertosQuery(), CancellationToken.None));
    }

    [Fact]
    public async Task GetReservationsByFlightAndPassengerUseTheirExistingQueries()
    {
        var queries = Substitute.For<IReservaQueries>();
        queries.GetByVueloAsync(4, Arg.Any<CancellationToken>()).Returns((IReadOnlyList<ReservaDto>)[]);
        queries.GetByPasajeroAsync(8, Arg.Any<CancellationToken>()).Returns((IReadOnlyList<ReservaDto>)[]);
        await new GetReservasByVueloQueryHandler(queries).Handle(new GetReservasByVueloQuery(4), CancellationToken.None);
        await new GetReservasByPasajeroQueryHandler(queries).Handle(new GetReservasByPasajeroQuery(8), CancellationToken.None);
        await queries.Received(1).GetByVueloAsync(4, Arg.Any<CancellationToken>());
        await queries.Received(1).GetByPasajeroAsync(8, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetSeatMapReturnsCapacityAndOccupiedSeats()
    {
        var queries = Substitute.For<IReservaQueries>();
        var map = new AsientosVueloDto(120, 20, ["1A", "3F"]);
        queries.GetAsientosAsync(9, Arg.Any<CancellationToken>()).Returns(map);
        var result = await new GetAsientosVueloQueryHandler(queries).Handle(new GetAsientosVueloQuery(9), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "1A", "3F" }, result.Value!.AsientosOcupados);
    }

    [Theory]
    [InlineData("1A", 120, true)]
    [InlineData("20F", 120, true)]
    [InlineData("21A", 120, false)]
    [InlineData("1G", 120, false)]
    public void SeatLayoutRulesRemainUnchanged(string seat, int capacity, bool valid) =>
        Assert.Equal(valid, AsientoLayout.EsValido(seat, capacity));

    [Fact]
    public async Task CreateReservationKeepsCurrentRulesAndPersistsOnce()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var passengers = Substitute.For<IGenericRepository<Pasajero>>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        var aircraft = Substitute.For<IGenericRepository<Aeronave>>();
        var routes = Substitute.For<IGenericRepository<Ruta>>();
        var reservations = Substitute.For<IGenericRepository<Reserva>>();
        unit.Pasajeros.Returns(passengers); unit.Vuelos.Returns(flights); unit.Aeronaves.Returns(aircraft);
        unit.Rutas.Returns(routes); unit.Reservas.Returns(reservations);
        passengers.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Pasajero { PasajeroId = 1, Nombres = "Ana", Apellidos = "Pérez", TipoDocumento = "DNI", NumeroDocumento = "12345678" });
        flights.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(new Vuelo { VueloId = 2, NumeroVuelo = "SP101", AeronaveId = 3, RutaId = 4, Estado = EstadoVuelo.Programado });
        aircraft.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(new Aeronave { AeronaveId = 3, Capacidad = 120 });
        routes.GetByIdAsync(4, Arg.Any<CancellationToken>()).Returns(new Ruta { RutaId = 4, CodigoOrigen = "LIM", CodigoDestino = "CUZ" });
        unit.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new CreateReservaCommandHandler(unit).Handle(new CreateReservaCommand(1, 2, " 1a "), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("1A", result.Value!.Asiento);
        await unit.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
