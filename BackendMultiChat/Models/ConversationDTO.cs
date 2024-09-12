namespace BackendMultiChat.Models
{
    public class ConversationDTO
    {
        public int ConversationId { get; set; }
        public string ConversationName { get; set; }
        public List<int> MemberIds { get; set; }
    }
}
