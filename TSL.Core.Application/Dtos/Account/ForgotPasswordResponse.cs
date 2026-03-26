
namespace TSL.Core.Application.Dtos.Account
{
    public class ForgotPasswordResponse
    {
        public bool HasError { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }
    }
}
