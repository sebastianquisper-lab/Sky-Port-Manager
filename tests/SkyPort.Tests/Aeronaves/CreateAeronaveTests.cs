using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Aeronaves.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;
namespace SkyPort.Tests.Aeronaves;
public sealed class CreateAeronaveTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(901)]
    public async Task RejectsInvalidCapacityWithoutSaving(int capacity)
    {
        var work = Substitute.For<IUnitOfWork>();
        var result = await new CreateAeronaveCommandHandler(work).Handle(new(1, "ABC", "Modelo", capacity, EstadoAeronave.Operativa), default);
        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await work.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task ValidAircraftIsPreparedAndCompletedWithCancellation()
    {
        var work = Substitute.For<IUnitOfWork>();
        var airlines = Substitute.For<IGenericRepository<Aerolinea>>();
        var aircraft = Substitute.For<IGenericRepository<Aeronave>>();
        work.Aerolineas.Returns(airlines);
        work.Aeronaves.Returns(aircraft);
        using var cts = new CancellationTokenSource();
        airlines.GetByIdAsync(1, cts.Token).Returns(new Aerolinea());
        var result = await new CreateAeronaveCommandHandler(work).Handle(new(1, " ABC ", " Modelo ", 180, EstadoAeronave.Operativa), cts.Token);
        Assert.True(result.IsSuccess);
        await aircraft.Received(1).AddAsync(Arg.Is<Aeronave>(x => x.Matricula == "ABC" && x.Modelo == "Modelo"), cts.Token);
        await work.Received(1).CompleteAsync(cts.Token);
    }
}
