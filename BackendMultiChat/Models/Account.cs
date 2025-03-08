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
        public RefreshToken? RefreshToken { get; set; }
        public ICollection<GroupMember> GroupMembers { get; set; }
        public enum Roles
        {
            Admin,      //1
            Manager,    //2
            Leader,     //3
            Staff       //4
        }

    }
}
