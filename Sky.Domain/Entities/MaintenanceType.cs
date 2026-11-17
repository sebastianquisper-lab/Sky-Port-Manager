using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class MaintenanceType
{
    public int MaintenanceTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<AircraftMaintenance> AircraftMaintenances { get; set; } = new List<AircraftMaintenance>();
}
