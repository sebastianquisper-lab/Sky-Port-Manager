using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Boletos;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Boletos.Queries;

namespace SkyPort.Tests.UseCases.Boletos;

public sealed class GetBoletosQueryHandlerTests
{
    [Fact]
    public async Task Handle_ClampsPageSizeAndReturnsEmptyPage()
    {
        var queries = Substitute.For<IBoletoQueries>();
        queries.GetPageAsync(1, 100, Arg.Any<CancellationToken>()).Returns(new PagedResult<BoletoDto>([], 1, 100, 0));
        var result = await new GetBoletosQueryHandler(queries).Handle(new GetBoletosQuery(0, 101), CancellationToken.None);
        Assert.Empty(result.Items);
        await queries.Received(1).GetPageAsync(1, 100, Arg.Any<CancellationToken>());
    }
}
