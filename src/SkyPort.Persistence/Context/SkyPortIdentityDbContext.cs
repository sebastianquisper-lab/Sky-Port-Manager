using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkyPort.Persistence.Identity;



namespace SkyPort.Persistence.Context;

public sealed class SkyPortIdentityDbContext(DbContextOptions<SkyPortIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser>(options);
