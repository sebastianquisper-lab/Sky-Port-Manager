using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Usuarios.Queries;

namespace SkyPort.Tests.Usuarios;

public sealed class GetUsuariosQueryTests
{
    [Fact]
    public async Task Handle_ClampsPaginationBeforeCallingIdentityPort()
    {
        var service = Substitute.For<IUsuarioIdentityService>();
        var expected = new PagedResult<UsuarioDto>(Array.Empty<UsuarioDto>(), 1, 100, 0);
        service.GetPageAsync(1, 100, Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new GetUsuariosQueryHandler(service);

        var result = await handler.Handle(new GetUsuariosQuery(0, 500), CancellationToken.None);

        Assert.Same(expected, result);
        await service.Received(1).GetPageAsync(1, 100, Arg.Any<CancellationToken>());
    }
}
