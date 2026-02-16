using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Users
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        public string? Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
