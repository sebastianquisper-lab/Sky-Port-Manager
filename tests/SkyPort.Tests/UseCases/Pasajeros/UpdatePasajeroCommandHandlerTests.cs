using NSubstitute;
using SkyPort.Application.UseCases.Pasajeros.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.UseCases.Pasajeros;

public sealed class UpdatePasajeroCommandHandlerTests
{
    private static UpdatePasajeroCommand Command(int id = 7) =>
        new(id, "DNI", "12345678", "Ana", "Pérez", new DateOnly(1990, 1, 1), "Peruana", null, null);

    [Fact]
    public async Task Handle_WhenPassengerDoesNotExist_ReturnsNotFoundWithoutSaving()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IGenericRepository<Pasajero>>();
        unit.Pasajeros.Returns(repo);

        var result = await new UpdatePasajeroCommandHandler(unit).Handle(Command(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDocumentBelongsToAnotherPassenger_ReturnsConflict()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IGenericRepository<Pasajero>>();
        unit.Pasajeros.Returns(repo);
        repo.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(new Pasajero { PasajeroId = 7 });
        repo.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Pasajero, bool>>>(), Arg.Any<CancellationToken>()).Returns(new Pasajero { PasajeroId = 8 });

        var result = await new UpdatePasajeroCommandHandler(unit).Handle(Command(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidChangesUpdatesTrackedPassengerAndCompletes()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IGenericRepository<Pasajero>>();
        var passenger = new Pasajero { PasajeroId = 7, TipoDocumento = "DNI", NumeroDocumento = "12345678" };
        unit.Pasajeros.Returns(repo);
        repo.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(passenger);
        repo.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Pasajero, bool>>>(), Arg.Any<CancellationToken>()).Returns((Pasajero?)null);
        unit.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new UpdatePasajeroCommandHandler(unit).Handle(Command(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Ana", passenger.Nombres);
        repo.DidNotReceive().Update(Arg.Any<Pasajero>());
        await unit.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
