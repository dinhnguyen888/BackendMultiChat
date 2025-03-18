namespace BackendMultiChat.Models
{
    public class ProjectMember
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

    }
}
