using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AircraftMaintenance
{
    public int MaintenanceId { get; set; }

    public int AircraftId { get; set; }

    public int MaintenanceTypeId { get; set; }

    public int AirportId { get; set; }

    public int? TechnicianEmployeeId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime StartDateUtc { get; set; }

    public DateTime? EndDateUtc { get; set; }

    public string? Notes { get; set; }

    public virtual Aircraft Aircraft { get; set; } = null!;

    public virtual Airport Airport { get; set; } = null!;

    public virtual MaintenanceType MaintenanceType { get; set; } = null!;

    public virtual Employee? TechnicianEmployee { get; set; }
}
