namespace BackendMultiChat.Dtos
{
    public class DMGetDto
    {
        public int DirectMessageId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string MessageText { get; set; }
        
    }

    public class DMPostDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string MessageText { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}
