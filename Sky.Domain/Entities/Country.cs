using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public string Iso2Code { get; set; } = null!;

    public string Iso3Code { get; set; } = null!;

    public virtual ICollection<Airline> Airlines { get; set; } = new List<Airline>();

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<EmployeeLicense> EmployeeLicenses { get; set; } = new List<EmployeeLicense>();

    public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
}
