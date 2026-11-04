using NSubstitute;
using SkyPort.Application.Dtos.Roles;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Roles.Queries;

namespace SkyPort.Tests.Roles;

public sealed class GetRolByIdQueryTests
{
    [Fact]
    public async Task Handle_ReturnsNotFoundForUnknownStringRoleId()
    {
        var service = Substitute.For<IRolIdentityService>();
        service.GetByIdAsync("missing-role", Arg.Any<CancellationToken>()).Returns((RolDetalleDto?)null);
        var handler = new GetRolByIdQueryHandler(service);

        var result = await handler.Handle(new GetRolByIdQuery("missing-role"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(SkyPort.Application.Common.ErrorType.NotFound, result.Error?.Type);
    }
}
