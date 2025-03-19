using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendMultiChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        // Get all TodoLists
        [HttpGet]
        public async Task<IActionResult> GetAllTodoLists()
        {
            try
            {
                var todoLists = await _todoService.GetAllTodoListsAsync();
                return Ok(todoLists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get TodoList by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoListById([FromQuery]int id)
        {
            try
            {
                var todoList = await _todoService.GetTodoListByIdAsync(id);
                if (todoList == null)
                {
                    return NotFound($"TodoList with ID {id} not found.");
                }
                return Ok(todoList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Create TodoList
        [HttpPost]
        public async Task<IActionResult> CreateTodoList([FromBody]TodoListPostDto dto)
        {
            try
            {
                var isCreated = await _todoService.CreateTodoListAsync(dto);
                if (!isCreated)
                {
                    return BadRequest("Failed to create TodoList.");
                }
                return Ok("TodoList created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Update TodoList
        [HttpPut]
        public async Task<IActionResult> UpdateTodoList([FromQuery]int id, [FromBody]TodoListUpdateDto dto)
        {
            try
            {
                var isUpdated = await _todoService.UpdateTodoListAsync(id, dto);
                if (!isUpdated)
                {
                    return NotFound($"TodoList with ID {id} not found or update failed.");
                }
                return Ok("TodoList updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Delete TodoList
        [HttpDelete]
        public async Task<IActionResult> DeleteTodoList([FromQuery] int id)
        {
            try
            {
                var isDeleted = await _todoService.DeleteTodoListAsync(id);
                if (!isDeleted)
                {
                    return NotFound($"TodoList with ID {id} not found.");
                }
                return Ok("TodoList deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
