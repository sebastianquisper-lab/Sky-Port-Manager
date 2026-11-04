using NSubstitute;
using SkyPort.Application.Dtos.Vuelos;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Vuelos.Queries;

namespace SkyPort.Tests.Vuelos;

public sealed class GetVueloByIdQueryTests
{
    [Fact]
    public async Task Handle_ReturnsFlightWithoutAssignedGate()
    {
        var flight = new VueloDetailDto(5, "SP105", 2, "LIM", "Lima", "CUZ", "Cusco", 4, "OB-1234", "A320", null, null,
            new DateTime(2027, 1, 2, 8, 0, 0), new DateTime(2027, 1, 2, 9, 30, 0), "Programado");
        var queries = Substitute.For<IVueloQueries>();
        queries.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(flight);
        var handler = new GetVueloByIdQueryHandler(queries);

        var result = await handler.Handle(new GetVueloByIdQuery(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value?.CodigoPuerta);
    }

    [Fact]
    public async Task Handle_ReturnsNotFoundForMissingFlight()
    {
        var queries = Substitute.For<IVueloQueries>();
        queries.GetByIdAsync(404, Arg.Any<CancellationToken>()).Returns((VueloDetailDto?)null);
        var handler = new GetVueloByIdQueryHandler(queries);

        var result = await handler.Handle(new GetVueloByIdQuery(404), CancellationToken.None);

        Assert.Equal(SkyPort.Application.Common.ErrorType.NotFound, result.Error?.Type);
    }
}
