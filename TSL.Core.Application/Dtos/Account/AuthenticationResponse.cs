using System.Text.Json.Serialization;

namespace TSL.Core.Application.Dtos.Account
{
    public class AuthenticationResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public List<string> Roles { get; set; } = new();
        public bool IsVerified { get; set; }

        public string JwtToken { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken {  get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? RefreshTokenExpiration { get; set; }

        public bool HasError { get; set; }

        public string? Error { get; set; }

    }
}
