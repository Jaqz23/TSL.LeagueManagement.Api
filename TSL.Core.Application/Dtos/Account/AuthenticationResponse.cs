using System.Text.Json.Serialization;

namespace TSL.Core.Application.Dtos.Account
{
    public class AuthenticationResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();


        public string JwtToken { get; set; } = string.Empty;

        [JsonIgnore]
        public string RefreshToken {  get; set; } = string.Empty;

        // Indica si la autenticacion fue exitosa
        public bool IsVerified { get; set; }

        public string? Error { get; set; }

        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
