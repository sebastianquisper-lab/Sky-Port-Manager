using System.ComponentModel.DataAnnotations;

namespace SkyPort.Web.Models;

public sealed class RolViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required, StringLength(256)]
    [Display(Name = "Nombre del rol")]
    public string Name { get; set; } = string.Empty;
}
