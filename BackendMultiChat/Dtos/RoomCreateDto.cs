using BackendMultiChat.Models;
using System.Text.Json.Serialization;

namespace BackendMultiChat.Dtos
{
    public class RoomCreateDto
    {
        [JsonIgnore]
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public ICollection<GroupMemberCreateDto>? GroupMembers { get; set; }
    }
}
