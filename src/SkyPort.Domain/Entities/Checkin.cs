using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Checkin
{
    public int CheckInId { get; set; }

    public int BoletoId { get; set; }

    public int? EmpleadoId { get; set; }

    public DateTime FechaHora { get; set; }

    public bool DocumentoVerificado { get; set; }

    public virtual Boleto Boleto { get; set; } = null!;

    public virtual Empleado? Empleado { get; set; }
}
