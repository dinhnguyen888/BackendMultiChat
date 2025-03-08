namespace BackendMultiChat.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string? RoomName { get; set; }

        // Navigation properties
        public ICollection<Message> Messages { get; set; }
        public ICollection<GroupMember> GroupMembers { get; set; }
        public ICollection<FileStorage>? Files { get; set; }
    }
}
