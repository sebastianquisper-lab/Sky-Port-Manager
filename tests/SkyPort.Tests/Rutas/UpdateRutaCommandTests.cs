using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Rutas.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Rutas;

public sealed class UpdateRutaCommandTests
{
    [Fact]
    public async Task Handle_ChangesOnlyDurationOnTrackedRoute()
    {
        var route = new Ruta
        {
            RutaId = 7,
            CodigoOrigen = "LIM",
            CiudadOrigen = "Lima",
            CodigoDestino = "AQP",
            CiudadDestino = "Arequipa",
            DuracionMinutos = 90
        };
        var (work, repository) = CreateWorkWithRoute(route);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new UpdateRutaCommandHandler(work).Handle(new(7, 100), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(100, route.DuracionMinutos);
        Assert.Equal("LIM", route.CodigoOrigen);
        Assert.Equal("Lima", route.CiudadOrigen);
        Assert.Equal("AQP", route.CodigoDestino);
        Assert.Equal("Arequipa", route.CiudadDestino);
        repository.DidNotReceive().Update(Arg.Any<Ruta>());
        await work.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ZeroAffectedRowsIsStillSuccessful()
    {
        var route = new Ruta { RutaId = 7, DuracionMinutos = 90 };
        var (work, _) = CreateWorkWithRoute(route);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(0);

        var result = await new UpdateRutaCommandHandler(work).Handle(new(7, 90), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnsNotFoundForMissingRoute()
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Ruta>>();
        work.Rutas.Returns(repository);
        repository.GetByIdAsync(404, Arg.Any<CancellationToken>()).Returns((Ruta?)null);

        var result = await new UpdateRutaCommandHandler(work).Handle(new(404, 90), CancellationToken.None);

        Assert.Equal(ErrorType.NotFound, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    private static (IUnitOfWork Work, IGenericRepository<Ruta> Repository) CreateWorkWithRoute(Ruta route)
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Ruta>>();
        work.Rutas.Returns(repository);
        repository.GetByIdAsync(route.RutaId, Arg.Any<CancellationToken>()).Returns(route);
        return (work, repository);
    }
}
