using TSL.Core.Application.Dtos.Account;
using TSL.Core.Application.Dtos.Common;

namespace TSL.Core.Application.Interfaces.Services
{
    public interface IAccountService
    {
        // Autentica un usuario con email y contraseña
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);

        // Registra un nuevo usuario en el sistema
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, string? origin = null);

        // Solicita un token de recuperacion de contraseña
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        // Restablece la contraseña de un usuario usando un token de recuperacion
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);

        // Refresca un token JWT expirado usando un refresh token valido
        Task<AuthenticationResponse> RefreshTokenAsync(string refreshToken);

        // Cierra la sesiOn del usuario revocando su refresh token
        Task<bool> LogoutAsync(string userId);

        // Confirma el email de un usuario usando un token de confirmacion
        Task<BaseResponseDto<string>> ConfirmEmailAsync(string userId, string token);

        // Obtiene informacion de un usuario por su ID
        Task<BaseResponseDto<UserDto>> GetUserByIdAsync(string userId);

        // Obtiene informacion de un usuario por su email
        Task<BaseResponseDto<UserDto>> GetUserByEmailAsync(string email);

        // Actualiza los datos de un usuario
        Task<BaseResponseDto<UserDto>> UpdateUserAsync(string userId, UpdateUserRequest request);

        // Cambia la contraseña de un usuario autenticado
        Task<BaseResponseDto<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);


    }
}
