
namespace TSL.Core.Application.Dtos.Account
{
    public class ResetPasswordResponse
    {
        public bool HasError { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; } 
    }
}
