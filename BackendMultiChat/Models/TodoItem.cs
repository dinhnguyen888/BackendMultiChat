namespace BackendMultiChat.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public DateTime DueAt { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public int TodoListId { get; set; }
        public TodoList TodoList { get; set; }
    }
}
