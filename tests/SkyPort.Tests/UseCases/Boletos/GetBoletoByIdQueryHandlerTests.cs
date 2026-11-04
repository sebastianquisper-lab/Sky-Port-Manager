using NSubstitute;
using SkyPort.Application.Dtos.Boletos;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Boletos.Queries;

namespace SkyPort.Tests.UseCases.Boletos;

public sealed class GetBoletoByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        var queries = Substitute.For<IBoletoQueries>();
        var result = await new GetBoletoByIdQueryHandler(queries).Handle(new GetBoletoByIdQuery(100), CancellationToken.None);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenFound_ReturnsTicketAndReferences()
    {
        var queries = Substitute.For<IBoletoQueries>();
        var dto = new BoletoDto(1, 2, "CODE-01", DateTime.Now, "Emitido", "R001", "Ana Pérez", "SP101", "1A");
        queries.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(dto);
        var result = await new GetBoletoByIdQueryHandler(queries).Handle(new GetBoletoByIdQuery(1), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal("R001", result.Value!.CodigoReserva);
    }
}
