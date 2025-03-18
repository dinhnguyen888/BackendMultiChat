using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace BackendMultiChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var tokenResponse = await _authService.LoginAsync(loginDto);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException uex)
            {
                return Unauthorized(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Refresh token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromQuery] Guid accountId,[FromQuery] string refreshToken)
        {
            try
            {
                var tokenResponse = await _authService.RefreshTokenAsync(accountId, refreshToken);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException uex)
            {
                return Unauthorized(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromQuery] string refreshToken)
        {
            try
            {
                var isLogout = await _authService.LogoutAsync(refreshToken);
                return Ok();
            }
            catch (UnauthorizedAccessException uex)
            {
                return Unauthorized(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
