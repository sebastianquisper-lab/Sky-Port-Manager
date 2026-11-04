using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Usuarios;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Identity;

namespace SkyPort.Infrastructure.Services;

public sealed class UsuarioIdentityService(UserManager<ApplicationUser> userManager) : IUsuarioIdentityService
{
    public async Task<PagedResult<UsuarioDto>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var users = userManager.Users.AsNoTracking();
        var totalCount = await users.CountAsync(cancellationToken);
        var items = await users
            .OrderBy(user => user.Email)
            .ThenBy(user => user.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new UsuarioDto(user.Id, user.Email, user.UserName, user.EmailConfirmed))
            .ToListAsync(cancellationToken);

        return new PagedResult<UsuarioDto>(items, page, pageSize, totalCount);
    }
}
