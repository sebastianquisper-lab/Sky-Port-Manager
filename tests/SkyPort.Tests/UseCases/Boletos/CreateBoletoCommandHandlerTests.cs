using NSubstitute;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Boletos.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Ports;

namespace SkyPort.Tests.UseCases.Boletos;

public sealed class CreateBoletoCommandHandlerTests
{
    private static (IUnitOfWork Unit, IGenericRepository<Reserva> Reservas, IGenericRepository<Boleto> Boletos) Setup()
    {
        var unit = Substitute.For<IUnitOfWork>();
        var reservas = Substitute.For<IGenericRepository<Reserva>>();
        var boletos = Substitute.For<IGenericRepository<Boleto>>();
        unit.Reservas.Returns(reservas);
        unit.Boletos.Returns(boletos);
        return (unit, reservas, boletos);
    }

    [Fact]
    public async Task Handle_ConfirmedReservationCreatesIssuedTicketAndCompletes()
    {
        var (unit, reservas, boletos) = Setup();
        reservas.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(new Reserva { ReservaId = 5, Estado = EstadoReserva.Confirmada });
        boletos.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Boleto, bool>>>(), Arg.Any<CancellationToken>()).Returns((Boleto?)null);
        Boleto? created = null;
        boletos.AddAsync(Arg.Do<Boleto>(b => created = b), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        unit.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await new CreateBoletoCommandHandler(unit).Handle(new CreateBoletoCommand(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(created);
        Assert.Equal(EstadoBoleto.Emitido, created!.Estado);
        Assert.Equal(14, created.CodigoBoleto.Length);
        Assert.Equal(DateTime.Today, created.FechaEmision.Date);
        await unit.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Handle_RejectsMissingOrCancelledReservation(bool exists)
    {
        var (unit, reservas, _) = Setup();
        if (exists) reservas.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(new Reserva { ReservaId = 5, Estado = EstadoReserva.Cancelada });

        var result = await new CreateBoletoCommandHandler(unit).Handle(new CreateBoletoCommand(5), CancellationToken.None);

        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenReservationAlreadyHasTicket_RejectsDuplicate()
    {
        var (unit, reservas, boletos) = Setup();
        reservas.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(new Reserva { ReservaId = 5, Estado = EstadoReserva.Confirmada });
        boletos.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Boleto, bool>>>(), Arg.Any<CancellationToken>()).Returns(new Boleto { BoletoId = 7 });
        var result = await new CreateBoletoCommandHandler(unit).Handle(new CreateBoletoCommand(5), CancellationToken.None);
        Assert.False(result.IsSuccess);
        await unit.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
}
