using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class FlightCrewAssignment
{
    public int FlightId { get; set; }

    public int EmployeeId { get; set; }

    public string CrewPosition { get; set; } = null!;

    public DateTime AssignedAtUtc { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Flight Flight { get; set; } = null!;
}
