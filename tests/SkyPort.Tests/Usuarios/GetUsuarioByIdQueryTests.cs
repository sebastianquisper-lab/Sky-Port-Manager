using NSubstitute;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Queries;

namespace SkyPort.Tests.Usuarios;

public sealed class GetUsuarioByIdQueryTests
{
    [Fact]
    public async Task Handle_ReturnsNotFoundForUnknownStringIdentityId()
    {
        var service = Substitute.For<IUsuarioIdentityService>();
        service.GetByIdAsync("missing-id", Arg.Any<CancellationToken>()).Returns((UsuarioDetalleDto?)null);
        var handler = new GetUsuarioByIdQueryHandler(service);

        var result = await handler.Handle(new GetUsuarioByIdQuery("missing-id"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SkyPort.Application.Common.ErrorType.NotFound, result.Error?.Type);
        await service.Received(1).GetByIdAsync("missing-id", Arg.Any<CancellationToken>());
    }
}
