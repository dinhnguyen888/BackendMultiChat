namespace BackendMultiChat.Dtos
{
    public class TodoListGetDto
    {
        public int TodoListId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsComplete { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string PriorityLevel { get; set; }
        public List<TodoItemGetDto> TodoItems { get; set; }
    }

    public class TodoListPostDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string PriorityLevel { get; set; }
        public List<TodoItemPostDto>? TodoItems { get; set; }
    }

    public class TodoListUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PriorityLevel { get; set; }
        public bool? IsComplete { get; set; }

        public List<TodoItemPostDto>? AddItems { get; set; }

        public List<int>? RemoveItemIds { get; set; }
    }

}
