using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Rutas.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Rutas;

public sealed class CreateRutaCommandTests
{
    [Theory]
    [InlineData("LIM", "Lima", "LIM", "Lima", 60)]
    [InlineData("TOOLONG", "Lima", "AQP", "Arequipa", 60)]
    [InlineData("LIM", "Lima", "AQP", "Arequipa", 0)]
    public async Task Handle_RejectsInvalidRouteWithoutSaving(
        string originCode,
        string originCity,
        string destinationCode,
        string destinationCity,
        int duration)
    {
        var work = Substitute.For<IUnitOfWork>();

        var result = await new CreateRutaCommandHandler(work).Handle(
            new(originCode, originCity, destinationCode, destinationCity, duration),
            CancellationToken.None);

        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PreparesAndCompletesValidRouteOnce()
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Ruta>>();
        work.Rutas.Returns(repository);
        repository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Ruta, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Ruta?)null);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);
        var handler = new CreateRutaCommandHandler(work);

        var result = await handler.Handle(new(" lim ", " Lima ", " aqp ", " Arequipa ", 90), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await repository.Received(1).AddAsync(
            Arg.Is<Ruta>(route => route.CodigoOrigen == "LIM" && route.CiudadOrigen == "Lima" && route.CodigoDestino == "AQP"),
            Arg.Any<CancellationToken>());
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapsDuplicateKeyRaceToConflict()
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Ruta>>();
        work.Rutas.Returns(repository);
        repository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Ruta, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Ruta?)null);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new DuplicateKeyException("duplicate"));
        var handler = new CreateRutaCommandHandler(work);

        var result = await handler.Handle(new("LIM", "Lima", "AQP", "Arequipa", 90), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
    }
}
