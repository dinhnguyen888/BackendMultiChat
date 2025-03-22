using StackExchange.Redis;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackendMultiChat.Models
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Roles Role { get; set; }

        // Allow null for token
        public RefreshToken? RefreshToken { get; set; }

        // Navigation properties
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ProjectMember> ProjectMember { get; set; } = new List<ProjectMember>();
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<TodoList> TodoLists { get; set; } = new List<TodoList>();
        public ICollection<DirectMessage> SentMessages { get; set; } = new List<DirectMessage>();
        public ICollection<DirectMessage> ReceivedMessages { get; set; } = new List<DirectMessage>();

        public enum Roles
        {
            Admin,
            Leader, 
            Staff   
        }
    }
}
