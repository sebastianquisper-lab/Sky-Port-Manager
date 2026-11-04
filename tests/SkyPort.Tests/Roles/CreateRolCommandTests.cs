using NSubstitute;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Roles.Commands;

namespace SkyPort.Tests.Roles;

public sealed class CreateRolCommandTests
{
    [Fact]
    public async Task Handle_TrimsRoleName()
    {
        var service = Substitute.For<IRolIdentityService>();
        var expected = Result<RolDto>.Success(new RolDto("role-id", "Auditor"));
        service.CreateAsync("Auditor", Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new CreateRolCommandHandler(service);

        var result = await handler.Handle(new CreateRolCommand(" Auditor "), CancellationToken.None);

        Assert.Same(expected, result);
        await service.Received(1).CreateAsync("Auditor", Arg.Any<CancellationToken>());
    }
}
