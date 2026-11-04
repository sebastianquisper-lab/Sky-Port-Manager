using System.Linq.Expressions;
using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Aeronaves.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
namespace SkyPort.Tests.Aeronaves;
public sealed class DeleteAeronaveTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ProtectsReferencesIncludingRaceAtSave(bool detectedBeforeSave)
    {
        var work = Substitute.For<IUnitOfWork>();
        var aircraft = Substitute.For<IGenericRepository<Aeronave>>();
        var flights = Substitute.For<IGenericRepository<Vuelo>>();
        work.Aeronaves.Returns(aircraft); work.Vuelos.Returns(flights);
        aircraft.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Aeronave { AeronaveId = 1 });
        flights.CountAsync(Arg.Any<Expression<Func<Vuelo, bool>>>(), Arg.Any<CancellationToken>()).Returns(detectedBeforeSave ? 1 : 0);
        work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromException<int>(new ReferenceConflictException(new Exception())));
        var result = await new DeleteAeronaveCommandHandler(work).Handle(new(1), default);
        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        if (detectedBeforeSave) aircraft.DidNotReceive().Remove(Arg.Any<Aeronave>());
    }
}
