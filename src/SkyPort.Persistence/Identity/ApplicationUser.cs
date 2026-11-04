using Microsoft.AspNetCore.Identity;

namespace SkyPort.Persistence.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;

}
