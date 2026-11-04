using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Models;

public sealed class CheckInViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un boleto.")]
    public int BoletoId { get; set; }

    public int? EmpleadoId { get; set; }

    public bool DocumentoVerificado { get; set; } = true;

    public IReadOnlyList<SelectListItem> Boletos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Empleados { get; set; } = [];
}
