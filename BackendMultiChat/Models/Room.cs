namespace BackendMultiChat.Models
{
    public class Room
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<FileStorage>? FileStorages { get; set; } = new List<FileStorage>();
    }
}
