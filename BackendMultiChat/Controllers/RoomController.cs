using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BackendMultiChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRooms();
                return Ok(rooms);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewRoom(RoomCreateDto dto)
        {
            try
            {
                var result = await _roomService.CreateNewRoom(dto);
                if (result)
                {
                    return Ok(new { message = "Room created successfully" });
                }
                return BadRequest(new { message = "Room creation failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRoomByUserId(Guid userId)
        {
            try
            {
                var rooms = await _roomService.GetRoomByUserId(userId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }



    }
}
