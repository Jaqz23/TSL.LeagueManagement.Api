using System.ComponentModel.DataAnnotations;

namespace TSL.Infrastructure.Identity.Settings
{
    public class JWTSettings
    {
        [Required(ErrorMessage ="La clave JWT es requerida")]
        [MinLength(32, ErrorMessage = "La clave JWT debe tener al menos 32 caracteres para ser segura")]
        public string Key { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El emisor (Issuer) es requerido")]
        public string Issuer { get; set; } = string.Empty;

        [Required(ErrorMessage = "La audiencia (Audience) es requerido")]
        public string Audience { get; set; } = string.Empty ;

        [Range(1, 1440, ErrorMessage = "La duración debe estar entre 1 y 1440 minutos (24 horas)")]
        public int DurationInMinutes { get; set; } = 120;

        [Range(1, 90, ErrorMessage = "La duración del Refresh Token debe estar entre 1 y 90 días")]
        public int RefreshTokenDurationInDays { get; set; } = 7;

        public bool ValidateLifetime { get; set; } = true;

        public bool ValidateIssuer { get; set; } = true;

        public bool ValidateAudience { get; set; } = true;

        public bool ValidateIssuerSigningKey { get; set; } = true;

        public int ClockSkewInMinutes { get; set; } = 5;

        // Valida que la configuración JWT sea valida
        public bool IsValid() 
        {
            return !string.IsNullOrWhiteSpace(Key) &&
                Key.Length >= 32 &&
                !string.IsNullOrWhiteSpace(Issuer) &&
                !string.IsNullOrWhiteSpace(Audience) &&
                DurationInMinutes > 0 &&
                RefreshTokenDurationInDays > 0;
        }

        // Obtiene los errores de validacion de la configuracion
        public List<string> GetValidationsErros() 
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Key))
                errors.Add("La clave JWT no puede estar vacia");
            else if (Key.Length < 32)
                errors.Add($"La clave JWT debe tener al menos 32 caracteres (actual: {Key.Length})");

            if (string.IsNullOrWhiteSpace(Issuer))
                errors.Add("El Issuer no puede estar vacio");

            if (string.IsNullOrWhiteSpace(Audience))
                errors.Add("Audience no puede estar vacia");

            if (DurationInMinutes <= 0)
                errors.Add("La duracion del token debe ser mayor a 0 minutos");

            if (RefreshTokenDurationInDays <= 0)
                errors.Add("La duracion del Refresh Token debe ser mayor a 0 dias");

            return errors;

        }

    }
}
