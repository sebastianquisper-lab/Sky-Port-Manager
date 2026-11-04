using NSubstitute;
using SkyPort.Application.Dtos.Rutas;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Rutas.Queries;

namespace SkyPort.Tests.Rutas;

public sealed class GetRutaByIdQueryTests
{
    [Fact]
    public async Task Handle_ReturnsRouteWhenIdExists()
    {
        var route = new RutaDto(5, "LIM", "Lima", "AQP", "Arequipa", 90);
        var queries = Substitute.For<IRutaQueries>();
        queries.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(route);
        var handler = new GetRutaByIdQueryHandler(queries);

        var result = await handler.Handle(new GetRutaByIdQuery(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(route, result.Value);
    }

    [Fact]
    public async Task Handle_ReturnsNotFoundWhenIdDoesNotExist()
    {
        var queries = Substitute.For<IRutaQueries>();
        queries.GetByIdAsync(500, Arg.Any<CancellationToken>()).Returns((RutaDto?)null);
        var handler = new GetRutaByIdQueryHandler(queries);

        var result = await handler.Handle(new GetRutaByIdQuery(500), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SkyPort.Application.Common.ErrorType.NotFound, result.Error?.Type);
    }
}
