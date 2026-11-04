using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Models;

public sealed class PuertaViewModel
{
    public int PuertaId { get; set; }

    [Range(1, int.MaxValue)]
    public int TerminalId { get; set; }

    [Required, StringLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indica si la puerta está disponible.")]
    public bool? Disponible { get; set; }

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Terminales { get; set; } = [];
}
