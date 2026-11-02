using System.ComponentModel.DataAnnotations;
using SkyPort.Application.Common.Constants;

namespace SkyPort.Web.Models.Users;

public sealed class UsuarioViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [RegularExpression($"^({Roles.Administrador}|{Roles.OperadorVuelos}|{Roles.AgenteCheckIn})$")]
    public string Role { get; set; } = string.Empty;

    public bool Activo { get; set; }
}
