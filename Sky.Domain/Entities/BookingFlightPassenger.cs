using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class BookingFlightPassenger
{
    public int BookingId { get; set; }

    public int FlightId { get; set; }

    public int PassengerId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Flight Flight { get; set; } = null!;

    public virtual Passenger Passenger { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
