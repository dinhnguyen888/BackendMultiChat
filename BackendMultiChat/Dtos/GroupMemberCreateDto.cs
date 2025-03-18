using BackendMultiChat.Models;

namespace BackendMultiChat.Dtos
{
    public class GroupMemberCreateDto
    {
        public Guid AccountId { get; set; }
        public DateTime JoinedDateTime { get; set; }
        public DateTime? LeftDateTime { get; set; } 
    }
}
