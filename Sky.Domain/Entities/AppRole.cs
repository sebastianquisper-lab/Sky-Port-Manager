using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AppRole
{
    public int AppRoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
}
