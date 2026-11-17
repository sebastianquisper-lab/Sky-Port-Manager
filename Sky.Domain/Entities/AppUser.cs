using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AppUser
{
    public int UserId { get; set; }

    public int AppRoleId { get; set; }

    public int? EmployeeId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? LastLoginUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public virtual AppRole AppRole { get; set; } = null!;

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual Employee? Employee { get; set; }
}
