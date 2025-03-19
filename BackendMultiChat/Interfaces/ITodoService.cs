using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface ITodoService
    {
        Task<bool> CreateTodoListAsync(TodoListPostDto dto);
        Task<bool> DeleteTodoListAsync(int id);
        Task<IEnumerable<TodoListGetDto>> GetAllTodoListsAsync();
        Task<TodoListGetDto?> GetTodoListByIdAsync(int id);
        Task<bool> UpdateTodoListAsync(int id, TodoListUpdateDto dto);
    }
}