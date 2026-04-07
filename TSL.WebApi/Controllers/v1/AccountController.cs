using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSL.Core.Application.Dtos.Account;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Interfaces.Services;

namespace TSL.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        #region Public Endpoints

        [HttpPost("authenticate")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request) 
        {
            var response = await _accountService.AuthenticateAsync(request);

            if (response.HasError)
            {
                // Credenciales incorrectas
                return Unauthorized(response);
            }

            return Ok(response);
        }

        // Refresca un token JWT expirado usando un refresh token valido
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request) 
        {
            var response = await _accountService.RefreshTokenAsync(request.RefreshToken);

            if (response.HasError) 
            {
                if (response.Error?.Contains("Token") == true) 
                {
                    return Unauthorized(response);
                }

                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request) 
        {
            var origin = Request.Headers["origin"].FirstOrDefault() ??
                        $"{Request.Scheme}://{Request.Host}";

            var response = await _accountService.RegisterAsync(request, origin);

            if (response.HasError) 
            {
                // Email o username ya existe
                if (response.Error?.Contains("ya está") == true ||
                    response.Error?.Contains("ya registrado") == true)
                {
                    return Conflict(response);
                }

                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status201Created, response);

        }

        #endregion


        #region Authenticated Endpoints

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponseDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout() 
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(BaseResponseDto<string>.ErrorResponse(
                    "No autenticado",
                    "No se pudo obtener el ID del usuario del token"
                ));
            }

            var result = await _accountService.LogoutAsync(userId);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BaseResponseDto<string>.ErrorResponse(
                        "Error al cerrar sesión",
                        "No se pudo cerrar la sesión correctamente"
                    ));
            }

            return Ok(BaseResponseDto<string>.SuccessResponse(
                "Sesión cerrada exitosamente",
                "Se han revocado todos los tokens activos"
            ));

        }


        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponseDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request) 
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            if (string.IsNullOrEmpty(userId)) 
            {
                return Unauthorized(BaseResponseDto<string>.ErrorResponse(
                    "No autenticado",
                    "No se pudo obtener el ID del usuario del token"
                ));
            }

            var result = await _accountService.ChangePasswordAsync(userId, request);

            return HandleServiceResponse(result);

        }

        #endregion


        #region Admin Endpoints

        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request) 
        {
            var origin = Request.Headers["origin"].FirstOrDefault() ??
                         $"{Request.Scheme}://{Request.Host}";

            var response = await _accountService.RegisterAdminAsync(request, origin);

            if (response == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new RegisterResponse
                {
                    HasError = true,
                    Error = "Error inesperado al procesar el registro"
                });
            }

            if (response.HasError) 
            {
                if (response.Error?.Contains("ya está") == true || 
                    response.Error?.Contains("ya registrado") == true) 
                {
                    return Conflict(response);
                }

                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status201Created, response);

        }


        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] string userId) 
        {
            var result = await _accountService.GetUserByIdAsync(userId);

            return HandleServiceResponse(result);
        }


        [HttpGet("by-email/{email}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponseDto<UserDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email) 
        {
            var result = await _accountService.GetUserByEmailAsync(email);

            return HandleServiceResponse(result);
        }

        #endregion

    }
}
