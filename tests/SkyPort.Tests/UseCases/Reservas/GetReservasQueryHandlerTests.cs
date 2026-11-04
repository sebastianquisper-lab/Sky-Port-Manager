using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Reservas.Dtos;
using SkyPort.Application.UseCases.Reservas.Queries;

namespace SkyPort.Tests.UseCases.Reservas;

public sealed class GetReservasQueryHandlerTests
{
    [Fact]
    public async Task Handle_ClampsPaginationAndReturnsEmptyPage()
    {
        var queries = Substitute.For<IReservaQueries>();
        var empty = new PagedResult<ReservaDto>([], 1, 100, 0);
        queries.GetPageAsync(1, 100, Arg.Any<CancellationToken>()).Returns(empty);

        var result = await new GetReservasQueryHandler(queries)
            .Handle(new GetReservasQuery(0, 200), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(1, result.PageNumber);
        await queries.Received(1).GetPageAsync(1, 100, Arg.Any<CancellationToken>());
    }
}
