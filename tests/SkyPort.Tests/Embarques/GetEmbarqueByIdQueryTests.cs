using NSubstitute;
using SkyPort.Application.Dtos.Embarques;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Embarques.Queries;

namespace SkyPort.Tests.Embarques;

public sealed class GetEmbarqueByIdQueryTests
{
    [Fact]
    public async Task Handle_ReturnsExistingBoardingWithNullableGate()
    {
        var boarding = new EmbarqueDetailDto(1, "TICKET123", "Pasajero Uno", "SP101", null, new DateTime(2027, 1, 1, 8, 0, 0));
        var queries = Substitute.For<IEmbarqueQueries>();
        queries.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(boarding);
        var handler = new GetEmbarqueByIdQueryHandler(queries);

        var result = await handler.Handle(new GetEmbarqueByIdQuery(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value?.Puerta);
    }

    [Fact]
    public async Task Handle_ReturnsNotFoundWhenBoardingDoesNotExist()
    {
        var queries = Substitute.For<IEmbarqueQueries>();
        queries.GetByIdAsync(404, Arg.Any<CancellationToken>()).Returns((EmbarqueDetailDto?)null);
        var handler = new GetEmbarqueByIdQueryHandler(queries);

        var result = await handler.Handle(new GetEmbarqueByIdQuery(404), CancellationToken.None);

        Assert.Equal(SkyPort.Application.Common.ErrorType.NotFound, result.Error?.Type);
    }
}
