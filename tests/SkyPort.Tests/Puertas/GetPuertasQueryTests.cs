using NSubstitute;
using SkyPort.Application.Dtos.Puertas;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Puertas.Queries;

namespace SkyPort.Tests.Puertas;

public sealed class GetPuertasQueryTests
{
    [Fact]
    public async Task DetailsReturnNotFoundWhenGateDoesNotExist()
    {
        var queries = Substitute.For<IPuertaQueries>();
        queries.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((PuertaDto?)null);

        var result = await new GetPuertaByIdQueryHandler(queries).Handle(new(404), CancellationToken.None);

        Assert.Equal(ErrorType.NotFound, result.Error?.Type);
    }

    [Fact]
    public async Task DetailsRetainTerminalLabelAndAvailability()
    {
        var queries = Substitute.For<IPuertaQueries>();
        var gate = new PuertaDto(5, 2, "Nacional", "B4", false);
        queries.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(gate);

        var result = await new GetPuertaByIdQueryHandler(queries).Handle(new(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(gate, result.Value);
    }
    [Fact]
    public async Task ReturnsEmptyListFromQueryPort()
    {
        var queries = Substitute.For<IPuertaQueries>();
        queries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<PuertaDto>());

        var result = await new GetPuertasQueryHandler(queries).Handle(new(), CancellationToken.None);

        Assert.Empty(result);
        await queries.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PreservesTerminalIdentificationAndNullableAvailability()
    {
        var queries = Substitute.For<IPuertaQueries>();
        var row = new PuertaDto(7, 3, "Internacional", "A1", null);
        queries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[] { row });

        var result = await new GetPuertasQueryHandler(queries).Handle(new(), CancellationToken.None);

        Assert.Equal(row, Assert.Single(result));
    }
}
