using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TSL.Core.Application.Dtos.Account;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Enums;
using TSL.Infrastructure.Identity.Context;
using TSL.Infrastructure.Identity.Entities;
using TSL.Infrastructure.Identity.Settings;

namespace TSL.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IdentityContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            JWTSettings jwtSettings,
            IdentityContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        #region Authentication

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request) 
        {
            // Buscar usuario por email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) 
            {
                return new AuthenticationResponse
                {
                    HasError = true,
                    Error = "Las credenciales son incorrectas"
                };
            }

            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email)) 
            {
                return new AuthenticationResponse
                {
                    HasError = true,
                    Error = "Datos de usuario incompletos"
                };
            }

            // Validar contraseña
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: true // Bloquear despues de 5 intentos fallidos
            );

            if (result.Succeeded) 
            {
                // Generar JWT 
                var jwtSecurityToken = await GenerateJwtTokenAsync(user);
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                // Generar Refresh Token
                var refreshToken = GenerateRefreshToken(user.Id, GetIpAddress());

                // Guardar Refresh Token en BD
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);

                // Remover refresh tokens antiguos (mas de 7 dias)
                await RemoveOldRefreshTokensAsync(user);

                // Obtener roles
                var roles = await _userManager.GetRolesAsync(user);

                return new AuthenticationResponse 
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Nombre = user.FirstName,
                    Apellido = user.LastName,
                    Roles = roles.ToList(),
                    IsVerified = user.EmailConfirmed,
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.Expires,
                    HasError = false
                };

            }

            if (result.IsLockedOut) 
            {
                return new AuthenticationResponse 
                {
                    HasError = true,
                    Error = "Su cuenta ha sido bloqueada por múltiples intentos fallidos. Intente más tarde."
                };
            }

            return new AuthenticationResponse 
            {
                HasError = true,
                Error = "Las credenciales son incorrectas"
            };

        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string token) 
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u .RefreshTokens.Any(rt => rt.Token == token));

            if (user == null) 
            {
                return new AuthenticationResponse
                {
                    HasError = true,
                    Error = "Token inválido"
                };
            }

            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Email)) 
            {
                return new AuthenticationResponse
                {
                    HasError = true,
                    Error = "Datos de usuario incompletos"
                };
            }

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);

            if (!refreshToken.IsActive) 
            {
                return new AuthenticationResponse
                {
                    HasError = true,
                    Error = "Token inválido o expirado"
                };
            }

            // Revocar el token actual
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = GetIpAddress();

            // Generar nuevo Token
            var newRefreshToken = GenerateRefreshToken(user.Id, GetIpAddress());
            newRefreshToken.ReplacedByToken = refreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            // Remover Refresh tokens antiguos
            await RemoveOldRefreshTokensAsync(user);

            // Generar nuevo JWT
            var jwtSecurityToken = await GenerateJwtTokenAsync(user);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthenticationResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Nombre = user.FirstName,
                Apellido = user.LastName,
                Roles = roles.ToList(),
                IsVerified = user.EmailConfirmed,
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.Expires,
                HasError = false
            };

        }

        public async Task<bool> LogoutAsync(string userId) 
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            // Revocar todos los refresh tokens activos
            var activeTokens = user.RefreshTokens.Where(rt => rt.IsActive).ToList();
            foreach (var token in activeTokens) 
            {
                token.Revoked = DateTime.UtcNow;
                token.RevokedByIp = GetIpAddress();
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync();

            return true;

        }

        #endregion


        #region Registration

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, string? origin = null) 
        {
            return await RegisterUserWithRoleAsync(request, Roles.User, origin);
        }

        public async Task<RegisterResponse> RegisterAdminAsync(RegisterRequest request, string? origin = null)
        {
            return await RegisterUserWithRoleAsync(request, Roles.Admin, origin);
        }

        #endregion


        #region Password Management

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string origin) 
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) 
            {
                return new ForgotPasswordResponse 
                {
                    HasError = false,
                    Error = "Si el email existe, recibirás un correo con instrucciones"
                };
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                return new ForgotPasswordResponse
                {
                    HasError = false,
                    Message = "Si el email existe, recibirás un correo con instrucciones"
                };
            }

            // Generar token de reset
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Construir URI de reset
            var resetUri = $"{origin}/reset-password?token={encodedToken}&email={user.Email}";

            // TODO: Implementar cuando tenga servicio de email
            // await _emailService.SendEmailAsync(new EmailRequest
            // {
            //     To = user.Email,
            //     Subject = "Restablecer contraseña - TSL",
            //     Body = $"Para restablecer tu contraseña, haz clic aquí: {resetUri}"
            // });

            // Temporal
            Console.WriteLine($"Reset Password Link: {resetUri}");

            return new ForgotPasswordResponse
            {
                HasError = false,
                Message = "Si el email existe, recibirás un correo con instrucciones"
            };

        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new ResetPasswordResponse
                {
                    HasError = true,
                    Error = "Token inválido o expirado"
                };
            }

            // Decodificar token
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            // Resetear password
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);

            if (!result.Succeeded)
            {
                return new ResetPasswordResponse
                {
                    HasError = true,
                    Error = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new ResetPasswordResponse
            {
                HasError = false,
                Message = "Contraseña restablecida exitosamente"
            };
        }

        public async Task<BaseResponseDto<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BaseResponseDto<string>.ErrorResponse(
                    "Usuario no encontrado",
                    $"No existe un usuario con ID {userId}"
                );
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BaseResponseDto<string>.ErrorResponse(
                    "Error al cambiar contraseña",
                     errors
                );
            }

            return BaseResponseDto<string>.SuccessResponse(
                "Contraseña cambiada exitosamente",
                "La contraseña ha sido actualizada correctamente"
            );
        }

        #endregion


        #region User Management

        public async Task<BaseResponseDto<string>> ConfirmEmailAsync(string userId, string token) 
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            {
                return BaseResponseDto<string>.ErrorResponse(
                    "Usuario no encontrado",
                    "El usuario especificado no existe"
                );
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BaseResponseDto<string>.ErrorResponse(
                    "Error al confirmar email",
                     errors
                );
            }

            return BaseResponseDto<string>.SuccessResponse(
                "Email confirmado exitosamente",
                "Tu cuenta ha sido verificada correctamente"
            );

        }

        public async Task<BaseResponseDto<UserDto>> GetUserByIdAsync(string userId) 
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            {
                return BaseResponseDto<UserDto>.ErrorResponse(
                    "Usuario no encontrado",
                    $"No existe un usuario con ID {userId}"
                );
            }

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName)) 
            {
                return BaseResponseDto<UserDto>.ErrorResponse(
                    "Datos incompletos",
                    "El usuario tiene datos incompletos"
                );
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto 
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Nombre = user.FirstName,
                Apellido = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };

            return BaseResponseDto<UserDto>.SuccessResponse(
                userDto,
                "Usuario obtenido exitosamente"
            );

        }

        public async Task<BaseResponseDto<UserDto>> GetUserByEmailAsync(string email) 
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BaseResponseDto<UserDto>.ErrorResponse(
                    "Usuario no encontrado",
                    $"No existe un usuario con email {email}"
                );
            }

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName))
            {
                return BaseResponseDto<UserDto>.ErrorResponse(
                    "Datos incompletos",
                    "El usuario tiene datos incompletos"
                );
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Nombre = user.FirstName,
                Apellido = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };

            return BaseResponseDto<UserDto>.SuccessResponse(
                userDto,
                "Usuario obtenido exitosamente"
            );

        }

        public async Task<BaseResponseDto<UserDto>> UpdateUserAsync(string userId, UpdateUserRequest request) 
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            {
                return BaseResponseDto<UserDto>.ErrorResponse(
                    "Usuario no encontrado", 
                    $"No existe un usuario con ID {userId}"
                );
            }

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName))
            {
                return BaseResponseDto<UserDto>.ErrorResponse(
                    "Datos incompletos",
                    "El usuario tiene datos incompletos"
                );
            }

            // Actualizar solo los campos permitidos
            user.FirstName = request.Nombre;
            user.LastName = request.Apellido;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) 
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BaseResponseDto<UserDto>.ErrorResponse(
                    "Error al actualizar usuario",
                    errors  
                );
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto 
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Nombre = user.FirstName,
                Apellido = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };

            return BaseResponseDto<UserDto>.SuccessResponse(
                userDto,
                "Usuario actualizado exitosamente"
            );

        }

        #endregion


        #region Private Methods

        private async Task<JwtSecurityToken> GenerateJwtTokenAsync(ApplicationUser user) 
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email)) 
            {
                throw new InvalidOperationException("El usuario no tiene UserName o Email configurado");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("nombre", user.FirstName),
                new Claim("apellido", user.LastName)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signinCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signinCredentials
            );

            return jwtSecurityToken;

        }


        private RefreshToken GenerateRefreshToken(string userId, string ipAddress) 
        {
            return new RefreshToken 
            {
                Token = GenerateRandomToken(),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                ApplicationUserId = userId
            };
        }


        private static string GenerateRandomToken(int size = 64) 
        {
            var randomBytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToBase64String(randomBytes);
        }


        private async Task RemoveOldRefreshTokensAsync(ApplicationUser user) 
        {
            // Remover tokens que:
            // 1. Ya expiraron hace mas de X dias
            // 2. Fueron revocados hace mas de X dias
            var tokensToRemove = user.RefreshTokens
                .Where(rt =>
                    (!rt.IsActive && rt.Expires.AddDays(_jwtSettings.RefreshTokenDurationInDays) <= DateTime.UtcNow) ||
                    (rt.Revoked.HasValue && rt.Revoked.Value.AddDays(_jwtSettings.RefreshTokenDurationInDays) <= DateTime.UtcNow)
                )
                    .ToList();

            if (tokensToRemove.Any()) 
            {
                foreach (var token in tokensToRemove) 
                {
                    user.RefreshTokens.Remove(token);
                }

                await _userManager.UpdateAsync(user);

            }

        }


        private string GetIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return "0.0.0.0";

            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor)) 
            {
                var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (ips.Length > 0) 
                {
                    return ips[0].Trim();
                }
            }

            // Obtener IP directa
            var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
            if (remoteIpAddress != null) 
            {
                // Si es IPv6 localhost, convertir a IPv4
                if (remoteIpAddress.ToString() == "::1") 
                {
                    return "127.0.0.1";
                }

                return remoteIpAddress.ToString();

            }

            return "0.0.0.0";

        }


        private async Task<RegisterResponse> RegisterUserWithRoleAsync(RegisterRequest request, Roles role, string? origin = null) 
        {
            // Validar que el username sea unico
            var existingUserByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (existingUserByUsername != null) 
            {
                return new RegisterResponse
                {
                    HasError = true,
                    Error = "El nombre de usuario ya está en uso"
                };
            }

            // Validar que el email sea unico
            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingUserByEmail != null) 
            {
                return new RegisterResponse
                {
                    HasError = true,
                    Error = "El email ya registrado"
                };
            }

            // Crear el usuario
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                FirstName = request.Nombre,
                LastName = request.Apellido,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = role != Roles.User // true para Admin, false para User
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) 
            {
                return new RegisterResponse 
                {
                    HasError = true,
                    Error = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Asignar rol especificado
            await _userManager.AddToRoleAsync(user, role.ToString());

            // TODO: Enviar email de confirmación para Users
            // if (role == Roles.User && !string.IsNullOrEmpty(origin))
            // {
            //     await SendVerificationEmailAsync(user, origin);
            // }

            return new RegisterResponse 
            {
                UserId = user.Id,
                HasError = false
            };

        }

        #endregion

    }
}
