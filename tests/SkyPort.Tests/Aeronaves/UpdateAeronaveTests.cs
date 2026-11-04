using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Aeronaves.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;
namespace SkyPort.Tests.Aeronaves;
public sealed class UpdateAeronaveTests
{
    [Fact]
    public async Task CapacityChangeWithFlightsDoesNotMutateOrSaveAircraft()
    {
        var work = Substitute.For<IUnitOfWork>();
        var aircraft = Substitute.For<IGenericRepository<Aeronave>>();
        var airlines = Substitute.For<IGenericRepository<Aerolinea>>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        work.Aeronaves.Returns(aircraft); work.Aerolineas.Returns(airlines); work.Vuelos.Returns(flights);
        var row = new Aeronave { AeronaveId = 1, Capacidad = 180, Matricula = "ABC" };
        aircraft.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(row);
        airlines.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(new Aerolinea());
        flights.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(1);
        var result = await new UpdateAeronaveCommandHandler(work).Handle(new(1, 2, "DEF", "Modelo", 100, EstadoAeronave.Operativa), default);
        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        Assert.Equal(180, row.Capacidad); Assert.Equal("ABC", row.Matricula);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
}
