using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public int BookingStatusId { get; set; }

    public string BookingCode { get; set; } = null!;

    public DateTime BookingDateUtc { get; set; }

    public DateTime? ExpirationDateUtc { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public virtual ICollection<BookingFlightPassenger> BookingFlightPassengers { get; set; } = new List<BookingFlightPassenger>();

    public virtual BookingStatus BookingStatus { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
