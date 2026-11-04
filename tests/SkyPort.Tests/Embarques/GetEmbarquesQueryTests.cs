using NSubstitute;
using SkyPort.Application.Dtos.Embarques;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Embarques.Queries;

namespace SkyPort.Tests.Embarques;

public sealed class GetEmbarquesQueryTests
{
    [Fact]
    public async Task Handle_ReturnsBoardingRowsFromQueryPort()
    {
        IReadOnlyList<EmbarqueListItemDto> expected =
        [new(1, "TICKET123", "Pasajero Uno", "SP101", null, new DateTime(2027, 1, 1, 8, 0, 0))];
        var queries = Substitute.For<IEmbarqueQueries>();
        queries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new GetEmbarquesQueryHandler(queries);

        var result = await handler.Handle(new GetEmbarquesQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task Handle_AllowsEmptyBoardingList()
    {
        var queries = Substitute.For<IEmbarqueQueries>();
        queries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<EmbarqueListItemDto>());
        var handler = new GetEmbarquesQueryHandler(queries);

        var result = await handler.Handle(new GetEmbarquesQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
