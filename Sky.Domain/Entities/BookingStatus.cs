using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class BookingStatus
{
    public int BookingStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
