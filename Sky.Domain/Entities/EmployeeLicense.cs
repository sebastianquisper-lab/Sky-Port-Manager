using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class EmployeeLicense
{
    public int LicenseId { get; set; }

    public int EmployeeId { get; set; }

    public int IssuingCountryId { get; set; }

    public string LicenseType { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Country IssuingCountry { get; set; } = null!;
}
