

namespace TSL.Core.Application.Dtos.Account
{
    public class JwtResponse
    {
        public string? Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
