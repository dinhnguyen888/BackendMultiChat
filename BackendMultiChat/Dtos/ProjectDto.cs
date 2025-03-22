namespace BackendMultiChat.Dtos
{
    public class ProjectGetDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid OwnerId { get; set; }

    }

    public class ProjectPostDto
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public Guid OwnerId { get; set; }
        public List<Guid> MemberIds { get; set; } = new List<Guid>();
    }

    public class ProjectUpdateDto
    {
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }

    }
   
    public class MemberProgressDto
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public double ProgressPercentage { get; set; }
    }

    public class  ProjectGetSimpleDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

    }

}
