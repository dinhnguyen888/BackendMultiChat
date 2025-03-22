using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendMultiChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectMessageController : ControllerBase
    {
        private readonly IDirectMessageService _directMessageService;

        public DirectMessageController(IDirectMessageService directMessageService)
        {
            _directMessageService = directMessageService;
        }

        // Lấy tin nhắn giữa 2 người dùng
        [HttpGet("{senderId}/{receiverId}")]
        public async Task<IActionResult> GetAllDMAsync(Guid senderId, Guid receiverId)
        {
            try
            {
                var messages = await _directMessageService.GetAllDMAsync(senderId, receiverId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Gửi tin nhắn trực tiếp
        [HttpPost]
        public async Task<IActionResult> CreateDirectMessageAsync([FromBody] DMPostDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var message = await _directMessageService.CreateDirectMessageAsync(dto);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Xóa tin nhắn trực tiếp
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDirectMessageAsync(int id)
        {
            try
            {
                var result = await _directMessageService.DeleteDirectMessageAsync(id);
                if (!result)
                    return NotFound("Message not found or already deleted.");

                return Ok("Message deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
