namespace BackendMultiChat.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public Guid OwnerId { get; set; }

        // Navigation properties
        public Account Owner { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<TodoList> TodoLists { get; set; } = new List<TodoList>();
    }
}
