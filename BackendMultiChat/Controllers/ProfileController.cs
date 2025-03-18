using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMultiChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromHeader(Name = "Authorization")] string token,
            [FromQuery] string oldPassword, [FromQuery] string newPassword)
        {
            try
            {
                token = token?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Token is missing");

                var result = await _profileService.ChangePasswordAsync(token, oldPassword, newPassword);
                if (result)
                    return Ok(new { message = "Password changed successfully" });

                return BadRequest(new { message = "Failed to change password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(
            [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                token = token?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Token is missing");

                var profile = await _profileService.GetProfileInformation(token);
                if (profile != null)
                    return Ok(profile);

                return NotFound(new { message = "Profile not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromHeader(Name = "Authorization")] string token,
            [FromBody] AccountUpdateDto dto)
        {
            try
            {
                token = token?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Token is missing");

                var result = await _profileService.UpdateProfileInformation(token, dto);
                if (result)
                    return Ok(new { message = "Profile updated successfully" });

                return BadRequest(new { message = "Failed to update profile" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
