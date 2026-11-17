using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Terminal
{
    public int TerminalId { get; set; }

    public int AirportId { get; set; }

    public string TerminalCode { get; set; } = null!;

    public string? TerminalName { get; set; }

    public bool IsActive { get; set; }

    public virtual Airport Airport { get; set; } = null!;

    public virtual ICollection<BoardingGate> BoardingGates { get; set; } = new List<BoardingGate>();

    public virtual ICollection<CheckInCounter> CheckInCounters { get; set; } = new List<CheckInCounter>();
}
