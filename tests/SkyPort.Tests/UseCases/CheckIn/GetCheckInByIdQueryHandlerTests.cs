using NSubstitute;
using SkyPort.Application.Dtos.CheckIn;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.CheckIn.Queries;

namespace SkyPort.Tests.UseCases.CheckIn;

public sealed class GetCheckInByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        var queries = Substitute.For<ICheckInQueries>();
        var result = await new GetCheckInByIdQueryHandler(queries).Handle(new GetCheckInByIdQuery(99), CancellationToken.None);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenFound_IncludesNullableEmployeeAndVerification()
    {
        var queries = Substitute.For<ICheckInQueries>();
        var dto = new CheckInDto(2, 3, "TICKET", 4, "RES", "Ana Pérez", null, null, DateTime.Now, true);
        queries.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(dto);
        var result = await new GetCheckInByIdQueryHandler(queries).Handle(new GetCheckInByIdQuery(2), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.EmpleadoId);
        Assert.True(result.Value.DocumentoVerificado);
    }
}
