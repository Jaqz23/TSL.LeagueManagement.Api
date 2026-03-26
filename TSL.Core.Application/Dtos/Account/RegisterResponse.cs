
namespace TSL.Core.Application.Dtos.Account
{
    public class RegisterResponse
    {
        public string UserId { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string? Error { get; set; }
    }
}
