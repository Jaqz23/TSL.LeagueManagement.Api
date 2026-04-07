using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Account
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
