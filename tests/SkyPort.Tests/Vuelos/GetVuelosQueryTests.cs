using NSubstitute;
using SkyPort.Application.Dtos.Vuelos;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Vuelos.Queries;

namespace SkyPort.Tests.Vuelos;

public sealed class GetVuelosQueryTests
{
    [Fact]
    public async Task Handle_ReturnsFlightRowsFromQueryPort()
    {
        IReadOnlyList<VueloListItemDto> expected =
        [new(1, "SP101", "LIM · Lima → AQP · Arequipa", "OB-1234", null,
            new DateTime(2027, 1, 2, 8, 0, 0), new DateTime(2027, 1, 2, 9, 30, 0), "Programado")];
        var queries = Substitute.For<IVueloQueries>();
        queries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new GetVuelosQueryHandler(queries);

        var result = await handler.Handle(new GetVuelosQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task Handle_AllowsEmptyFlightList()
    {
        var queries = Substitute.For<IVueloQueries>();
        queries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<VueloListItemDto>());
        var handler = new GetVuelosQueryHandler(queries);

        var result = await handler.Handle(new GetVuelosQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
