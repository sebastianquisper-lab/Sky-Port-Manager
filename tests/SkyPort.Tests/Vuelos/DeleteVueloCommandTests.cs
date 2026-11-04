using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Vuelos.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Vuelos;

public sealed class DeleteVueloCommandTests
{
    [Fact]
    public async Task Handle_RejectsFlightsWithReservationsOrCrew()
    {
        var (work, repository, queries) = CreateWork(new Vuelo { VueloId = 9 });
        queries.HasReservationsOrCrewAsync(9, Arg.Any<CancellationToken>()).Returns(true);

        var result = await new DeleteVueloCommandHandler(work, queries).Handle(new(9), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        repository.DidNotReceive().Remove(Arg.Any<Vuelo>());
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RemovesUnreferencedFlight()
    {
        var flight = new Vuelo { VueloId = 9 };
        var (work, repository, queries) = CreateWork(flight);
        queries.HasReservationsOrCrewAsync(9, Arg.Any<CancellationToken>()).Returns(false);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new DeleteVueloCommandHandler(work, queries).Handle(new(9), CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Received(1).Remove(flight);
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapsForeignKeyRaceToConflict()
    {
        var (work, _, queries) = CreateWork(new Vuelo { VueloId = 9 });
        queries.HasReservationsOrCrewAsync(9, Arg.Any<CancellationToken>()).Returns(false);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new ReferenceConflictException(new InvalidOperationException()));

        var result = await new DeleteVueloCommandHandler(work, queries).Handle(new(9), CancellationToken.None);

        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
    }

    private static (IUnitOfWork Work, IGenericRepository<Vuelo> Repository, IVueloQueries Queries) CreateWork(Vuelo flight)
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Vuelo>>();
        var queries = Substitute.For<IVueloQueries>();
        work.Vuelos.Returns(repository);
        repository.GetByIdAsync(flight.VueloId, Arg.Any<CancellationToken>()).Returns(flight);
        return (work, repository, queries);
    }
}
