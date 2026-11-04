using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace SkyPort.Web.Models;
public sealed class TripulacionViewModel
{
    [Range(1, int.MaxValue)] public int VueloId { get; set; }
    [Range(1, int.MaxValue)] public int EmpleadoId { get; set; }
    [Required, StringLength(30)] public string RolAbordo { get; set; } = "";
    [ValidateNever] public IReadOnlyList<SelectListItem> Vuelos { get; set; } = [];
    [ValidateNever] public IReadOnlyList<SelectListItem> Empleados { get; set; } = [];
}
