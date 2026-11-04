using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Rutas.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Rutas;

public sealed class DeleteRutaCommandTests
{
    [Fact]
    public async Task Handle_RejectsRouteWithFlights()
    {
        var route = new Ruta { RutaId = 7 };
        var (work, repository) = CreateWork(route);
        var queries = Substitute.For<IRutaQueries>();
        queries.IsUsedByFlightsAsync(7, Arg.Any<CancellationToken>()).Returns(true);

        var result = await new DeleteRutaCommandHandler(work, queries).Handle(new(7), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        repository.DidNotReceive().Remove(Arg.Any<Ruta>());
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RemovesUnusedRouteAndCompletesOnce()
    {
        var route = new Ruta { RutaId = 7 };
        var (work, repository) = CreateWork(route);
        var queries = Substitute.For<IRutaQueries>();
        queries.IsUsedByFlightsAsync(7, Arg.Any<CancellationToken>()).Returns(false);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new DeleteRutaCommandHandler(work, queries).Handle(new(7), CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Received(1).Remove(route);
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapsForeignKeyRaceToConflict()
    {
        var route = new Ruta { RutaId = 7 };
        var (work, _) = CreateWork(route);
        var queries = Substitute.For<IRutaQueries>();
        queries.IsUsedByFlightsAsync(7, Arg.Any<CancellationToken>()).Returns(false);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new ReferenceConflictException(new InvalidOperationException()));

        var result = await new DeleteRutaCommandHandler(work, queries).Handle(new(7), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
    }

    private static (IUnitOfWork Work, IGenericRepository<Ruta> Repository) CreateWork(Ruta route)
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Ruta>>();
        work.Rutas.Returns(repository);
        repository.GetByIdAsync(route.RutaId, Arg.Any<CancellationToken>()).Returns(route);
        return (work, repository);
    }
}
