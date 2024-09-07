namespace BackendMultiChat.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public string? ConversationName { get; set; }

        // Navigation properties
        public ICollection<Message> Messages { get; set; }
        public ICollection<GroupMember> GroupMembers { get; set; }
        public ICollection<FileSaveInServer>? Files { get; set; }
    }
}
