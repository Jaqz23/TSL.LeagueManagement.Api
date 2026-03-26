using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Account
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;
    }
}
