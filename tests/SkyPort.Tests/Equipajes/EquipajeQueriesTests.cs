using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Equipajes;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Equipajes.Queries;

namespace SkyPort.Tests.Equipajes;

public sealed class EquipajeQueriesTests
{
    [Fact]
    public async Task ListKeepsWeightPrecisionStateAndTicketReference()
    {
        var queries = Substitute.For<IEquipajeQueries>();
        var row = new EquipajeDto(2, 9, "TCK-91", "BG-02", 31.75m, "Cargado");
        queries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[] { row });

        var result = await new GetEquipajesQueryHandler(queries).Handle(new(), CancellationToken.None);

        Assert.Equal(row, Assert.Single(result));
    }

    [Fact]
    public async Task DetailsDistinguishMissingBaggage()
    {
        var queries = Substitute.For<IEquipajeQueries>();
        queries.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((EquipajeDto?)null);

        var result = await new GetEquipajeByIdQueryHandler(queries).Handle(new(404), CancellationToken.None);

        Assert.Equal(ErrorType.NotFound, result.Error?.Type);
    }

    [Fact]
    public async Task DetailsPreserveDecimalWeightAndTicketCode()
    {
        var queries = Substitute.For<IEquipajeQueries>();
        var row = new EquipajeDto(2, 9, "TCK-91", "BG-02", 12.30m, "Registrado");
        queries.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(row);

        var result = await new GetEquipajeByIdQueryHandler(queries).Handle(new(2), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12.30m, result.Value?.PesoKg);
        Assert.Equal("TCK-91", result.Value?.CodigoBoleto);
    }
}
