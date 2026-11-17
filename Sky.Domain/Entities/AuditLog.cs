using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AuditLog
{
    public long AuditId { get; set; }

    public int? UserId { get; set; }

    public string TableName { get; set; } = null!;

    public string RecordId { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public virtual AppUser? User { get; set; }
}
