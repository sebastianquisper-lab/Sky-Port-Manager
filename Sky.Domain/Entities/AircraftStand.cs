using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AircraftStand
{
    public int StandId { get; set; }

    public int AirportId { get; set; }

    public string StandCode { get; set; } = null!;

    public bool IsRemote { get; set; }

    public bool IsActive { get; set; }

    public virtual Airport Airport { get; set; } = null!;

    public virtual ICollection<FlightStandAssignment> FlightStandAssignments { get; set; } = new List<FlightStandAssignment>();
}
