namespace BackendMultiChat.Models
{
    public class Room
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<RoomMessage> Messages { get; set; } = new List<RoomMessage>();
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<FileStorage>? FileStorages { get; set; } = new List<FileStorage>();
    }
}
