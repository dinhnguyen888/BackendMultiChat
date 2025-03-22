using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendMultiChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // Get all messages in a room
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetAllMessages(Guid roomId)
        {
            try
            {
                var messages = await _messageService.GetAllMessagesAsync(roomId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving messages: {ex.Message}");
            }
        }
        
        // Create a new message
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] RMPostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var message = await _messageService.CreateMessageAsync(dto);
                return CreatedAtAction(nameof(GetAllMessages), new { roomId = message.RoomId }, message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating message: {ex.Message}");
            }
        }

        // Delete a message
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                var success = await _messageService.DeleteMessageAsync(id);
                if (!success)
                {
                    return NotFound($"Message with id {id} not found.");
                }

                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting message: {ex.Message}");
            }
        }
    }
}
