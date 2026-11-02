using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SkyPort.Persistence.Context;

public sealed class SkyPortIdentityDbContext(DbContextOptions<SkyPortIdentityDbContext> options)
    : IdentityDbContext<IdentityUser>(options);
