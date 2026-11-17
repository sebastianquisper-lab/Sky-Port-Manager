using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class VLicensesExpiring
{
    public int? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? LicenseType { get; set; }

    public string? LicenseNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? DaysLeft { get; set; }
}
