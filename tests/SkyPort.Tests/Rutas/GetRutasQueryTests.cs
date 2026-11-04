using NSubstitute;
using SkyPort.Application.UseCases.Rutas.Queries;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.Rutas;

public sealed class GetRutasQueryTests
{
    [Fact]
    public async Task Handle_ReturnsRoutesInStableOriginDestinationIdOrder()
    {
        var routeRepository = Substitute.For<IGenericRepository<Ruta>>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Rutas.Returns(routeRepository);
        routeRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Ruta { RutaId = 4, CodigoOrigen = "LIM", CodigoDestino = "CUZ" },
            new Ruta { RutaId = 3, CodigoOrigen = "LIM", CodigoDestino = "CUZ" },
            new Ruta { RutaId = 2, CodigoOrigen = "AQP", CodigoDestino = "LIM" }
        });
        var handler = new GetRutasQueryHandler(unitOfWork);

        var result = await handler.Handle(new GetRutasQuery(), CancellationToken.None);

        Assert.Equal(new[] { 2, 3, 4 }, result.Select(route => route.RutaId));
    }

    [Fact]
    public async Task Handle_ReturnsEmptyListWhenRepositoryHasNoRoutes()
    {
        var routeRepository = Substitute.For<IGenericRepository<Ruta>>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Rutas.Returns(routeRepository);
        routeRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<Ruta>());
        var handler = new GetRutasQueryHandler(unitOfWork);

        var result = await handler.Handle(new GetRutasQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
