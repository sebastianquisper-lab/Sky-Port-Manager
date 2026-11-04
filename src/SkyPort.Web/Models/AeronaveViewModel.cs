using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkyPort.Application.Common.Constants;
namespace SkyPort.Web.Models;
public sealed class AeronaveViewModel
{
    public int AeronaveId { get; set; }
    [Range(1, int.MaxValue)] public int AerolineaId { get; set; }
    [Required, StringLength(20)] public string Matricula { get; set; } = "";
    [Required, StringLength(60)] public string Modelo { get; set; } = "";
    [Range(1, 900)] public int Capacidad { get; set; }
    [Required] public string Estado { get; set; } = EstadoAeronave.Operativa;
    [ValidateNever] public IReadOnlyList<SelectListItem> Aerolineas { get; set; } = [];
}
