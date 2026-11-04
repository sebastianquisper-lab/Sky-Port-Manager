using System.ComponentModel.DataAnnotations;

namespace SkyPort.Web.Models;

public sealed class TerminalViewModel
{
    public int TerminalId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre admite hasta 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}
