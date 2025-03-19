using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Services
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TodoService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create TodoList
        public async Task<bool> CreateTodoListAsync(TodoListPostDto dto)
        {
            var todoList = _mapper.Map<TodoList>(dto);

            if (dto.TodoItems != null)
            {
                todoList.TodoItems = dto.TodoItems
                    .Select(i => _mapper.Map<TodoItem>(i))
                    .ToList();
            }

            _context.TodoLists.Add(todoList);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all TodoLists
        public async Task<IEnumerable<TodoListGetDto>> GetAllTodoListsAsync()
        {
            var todoLists = await _context.TodoLists
                                          .Include(t => t.TodoItems)
                                          .ToListAsync();

            return _mapper.Map<IEnumerable<TodoListGetDto>>(todoLists);
        }

        // Get TodoList by Id
        public async Task<TodoListGetDto?> GetTodoListByIdAsync(int id)
        {
            var todoList = await _context.TodoLists
                                         .Include(t => t.TodoItems)
                                         .FirstOrDefaultAsync(t => t.TodoListId == id);

            return _mapper.Map<TodoListGetDto>(todoList);
        }

        // Update TodoList
        public async Task<bool> UpdateTodoListAsync(int id, TodoListUpdateDto dto)
        {
            var todoList = await _context.TodoLists
                                         .Include(t => t.TodoItems)
                                         .FirstOrDefaultAsync(t => t.TodoListId == id);

            if (todoList == null) return false;

            // Update TodoList properties
            _mapper.Map(dto, todoList);

            // Add new TodoItem to TodoList
            if (dto.AddItems != null)
            {
                foreach (var itemDto in dto.AddItems)
                {
                    var newItem = _mapper.Map<TodoItem>(itemDto);
                    newItem.TodoListId = id;
                    todoList.TodoItems.Add(newItem);
                }
            }

            // Delete TodoItem from TodoList
            if (dto.RemoveItemIds != null)
            {
                foreach (var itemId in dto.RemoveItemIds)
                {
                    var existingItem = todoList.TodoItems.FirstOrDefault(x => x.Id == itemId);
                    if (existingItem != null)
                    {
                        todoList.TodoItems.Remove(existingItem);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        // Delete TodoList
        public async Task<bool> DeleteTodoListAsync(int id)
        {
            var todoList = await _context.TodoLists.FindAsync(id);
            if (todoList == null) return false;

            _context.TodoLists.Remove(todoList);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
