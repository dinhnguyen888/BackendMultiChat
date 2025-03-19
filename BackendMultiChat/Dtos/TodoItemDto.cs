namespace BackendMultiChat.Dtos
{
    public class TodoItemGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public DateTime DueAt { get; set; }
        public string Description { get; set; }
    }

    public class TodoItemPostDto
    {
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public DateTime DueAt { get; set; }
        public string Description { get; set; }
    }

    public class TodoItemUpdateDto
    {
        public string? Title { get; set; }
        public bool? IsDone { get; set; }
        public DateTime? DueAt { get; set; }
        public string? Description { get; set; }
    }
}
