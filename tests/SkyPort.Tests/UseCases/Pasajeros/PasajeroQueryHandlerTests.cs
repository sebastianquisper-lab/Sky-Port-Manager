using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Pasajeros.Queries;

namespace SkyPort.Tests.UseCases.Pasajeros;

public sealed class PasajeroQueryHandlerTests
{
    [Fact]
    public async Task GetPasajeros_PreservesFilterAndClampsPagination()
    {
        var queries = Substitute.For<IPasajeroQueries>();
        var expected = new PagedResult<PasajeroDto>([], 1, 100, 0);
        queries.GetPageAsync(" Ana ", 1, 100, Arg.Any<CancellationToken>()).Returns(expected);

        var actual = await new GetPasajerosQueryHandler(queries)
            .Handle(new GetPasajerosQuery(" Ana ", 0, 500), CancellationToken.None);

        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task GetPasajeroById_ReturnsNotFoundForMissingPassenger()
    {
        var queries = Substitute.For<IPasajeroQueries>();
        var result = await new GetPasajeroByIdQueryHandler(queries)
            .Handle(new GetPasajeroByIdQuery(999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        await queries.Received(1).GetByIdAsync(999, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByDocument_PreservesLookupUsedByReservationCreation()
    {
        var queries = Substitute.For<IPasajeroQueries>();
        var pasajero = new PasajeroDto(1, "DNI", "12345678", "Ana", "Pérez", new DateOnly(1990, 1, 1), "PE", null, null);
        queries.GetByDocumentoAsync("DNI", "12345678", Arg.Any<CancellationToken>()).Returns(pasajero);

        var result = await new GetPasajeroByDocumentoQueryHandler(queries)
            .Handle(new GetPasajeroByDocumentoQuery(" DNI ", " 12345678 "), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(pasajero, result.Value);
    }
}
