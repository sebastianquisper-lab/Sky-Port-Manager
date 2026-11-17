using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class CabinClass
{
    public int CabinClassId { get; set; }

    public string ClassCode { get; set; } = null!;

    public string ClassName { get; set; } = null!;

    public virtual ICollection<AircraftSeat> AircraftSeats { get; set; } = new List<AircraftSeat>();

    public virtual ICollection<Fare> Fares { get; set; } = new List<Fare>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
