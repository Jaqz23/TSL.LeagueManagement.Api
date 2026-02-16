
namespace TSL.Core.Application.Dtos.Account
{
    public class RegisterResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
