using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class EmployeeRole
{
    public int EmployeeRoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
