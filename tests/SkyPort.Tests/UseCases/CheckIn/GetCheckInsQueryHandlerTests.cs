using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.CheckIn;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.CheckIn.Queries;

namespace SkyPort.Tests.UseCases.CheckIn;

public sealed class GetCheckInsQueryHandlerTests
{
    [Fact]
    public async Task Handle_EmptyPageKeepsNullableEmployeeAndVerifiesPagination()
    {
        var queries = Substitute.For<ICheckInQueries>();
        queries.GetPageAsync(1, 100, Arg.Any<CancellationToken>()).Returns(new PagedResult<CheckInDto>([], 1, 100, 0));
        var result = await new GetCheckInsQueryHandler(queries).Handle(new GetCheckInsQuery(0, 500), CancellationToken.None);
        Assert.Empty(result.Items);
        await queries.Received(1).GetPageAsync(1, 100, Arg.Any<CancellationToken>());
    }
}
