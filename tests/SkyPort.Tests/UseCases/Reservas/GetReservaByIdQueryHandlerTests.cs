using NSubstitute;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Reservas.Dtos;
using SkyPort.Application.UseCases.Reservas.Queries;

namespace SkyPort.Tests.UseCases.Reservas;

public sealed class GetReservaByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenReservationDoesNotExist_ReturnsNotFound()
    {
        var queries = Substitute.For<IReservaQueries>();
        var result = await new GetReservaByIdQueryHandler(queries).Handle(new GetReservaByIdQuery(999), CancellationToken.None);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenReservationIsCancelled_ReturnsItsDetails()
    {
        var queries = Substitute.For<IReservaQueries>();
        var reservation = new ReservaDto(12, "ABC123", 1, "Ana Pérez", "DNI", "12345678", 2, "SP101", "LIM - CUZ", DateTime.Now, "1A", DateTime.Now, "Cancelada", null, null);
        queries.GetByIdAsync(12, Arg.Any<CancellationToken>()).Returns(reservation);
        var result = await new GetReservaByIdQueryHandler(queries).Handle(new GetReservaByIdQuery(12), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal("Cancelada", result.Value!.Estado);
    }
}
