using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class BoardingGate
{
    public int GateId { get; set; }

    public int TerminalId { get; set; }

    public string GateCode { get; set; } = null!;

    public string GateType { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Boarding> Boardings { get; set; } = new List<Boarding>();

    public virtual ICollection<Flight> FlightArrivalGates { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDepartureGates { get; set; } = new List<Flight>();

    public virtual Terminal Terminal { get; set; } = null!;
}
