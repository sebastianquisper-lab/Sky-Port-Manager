using NSubstitute;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Pasajeros.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.UseCases.Pasajeros;

public sealed class CreatePasajeroCommandHandlerTests
{
    private static CreatePasajeroCommand ValidCommand(string number = "12345678") =>
        new("DNI", number, "Ana", "Pérez", new DateOnly(1990, 1, 1), "Peruana", null, null);

    [Fact]
    public async Task Handle_WithValidInput_PreparesAndCompletesPassenger()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IGenericRepository<Pasajero>>();
        unit.Pasajeros.Returns(repo);
        repo.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Pasajero, bool>>>(), Arg.Any<CancellationToken>()).Returns((Pasajero?)null);
        unit.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new CreatePasajeroCommandHandler(unit).Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await repo.Received(1).AddAsync(Arg.Is<Pasajero>(p => p.NumeroDocumento == "12345678"), Arg.Any<CancellationToken>());
        await unit.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateDocument_RejectsWithoutSaving()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IGenericRepository<Pasajero>>();
        unit.Pasajeros.Returns(repo);
        repo.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Pasajero, bool>>>(), Arg.Any<CancellationToken>()).Returns(new Pasajero());

        var result = await new CreatePasajeroCommandHandler(unit).Handle(ValidCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidDocument_RejectsBeforeRepositoryLookup()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var result = await new CreatePasajeroCommandHandler(unit)
            .Handle(ValidCommand("fake"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        _ = unit.DidNotReceive().Pasajeros;
    }
}
