
namespace TSL.Core.Application.Dtos.Account
{
    public class ResetPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
