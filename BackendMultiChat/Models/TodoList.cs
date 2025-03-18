using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackendMultiChat.Models
{
    public class TodoList
    {
        public int TodoListId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsComplete { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        // Foreign keys
        public Guid UserId { get; set; }
        public int ProjectId { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Account Account { get; set; }

        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

        [Column(TypeName = "nvarchar(20)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority PriorityLevel { get; set; }

        public enum Priority
        {
            High,
            Medium,
            Low
        }
    }
}
