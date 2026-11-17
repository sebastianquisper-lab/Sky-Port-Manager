using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AircraftStatus
{
    public int AircraftStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();
}
