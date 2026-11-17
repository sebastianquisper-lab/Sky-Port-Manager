using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Passenger
{
    public int PassengerId { get; set; }

    public int DocumentTypeId { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public int? NationalityCountryId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<BookingFlightPassenger> BookingFlightPassengers { get; set; } = new List<BookingFlightPassenger>();

    public virtual DocumentType DocumentType { get; set; } = null!;

    public virtual Country? NationalityCountry { get; set; }
}
