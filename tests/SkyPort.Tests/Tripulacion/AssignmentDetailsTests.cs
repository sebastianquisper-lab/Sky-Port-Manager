using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.TripulacionVuelo.Queries;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;
namespace SkyPort.Tests.Tripulacion;
public sealed class AssignmentDetailsTests
{
    [Fact]
    public async Task UsesBothKeysInModelOrderWithoutSaving()
    {
        var work = Substitute.For<IUnitOfWork>();
        var repository = Substitute.For<IGenericRepository<Tripulacionvuelo>>();
        work.TripulacionesVuelo.Returns(repository);
        using var cts = new CancellationTokenSource();
        repository.GetByKeysAsync(Arg.Is<object[]>(keys => keys.Length == 2 && (int)keys[0] == 7 && (int)keys[1] == 12), cts.Token)
            .Returns(new Tripulacionvuelo { VueloId = 7, EmpleadoId = 12, RolAbordo = "Piloto" });
        var result = await new GetTripulacionVueloByIdQueryHandler(work).Handle(new(7, 12), cts.Token);
        Assert.True(result.IsSuccess);
        Assert.Equal("Piloto", result.Value!.RolAbordo);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task MissingCombinationReturnsNotFound()
    {
        var work = Substitute.For<IUnitOfWork>();
        work.TripulacionesVuelo.Returns(Substitute.For<IGenericRepository<Tripulacionvuelo>>());
        var result = await new GetTripulacionVueloByIdQueryHandler(work).Handle(new(7, 12), default);
        Assert.Equal(ErrorType.NotFound, result.Error?.Type);
    }
}
