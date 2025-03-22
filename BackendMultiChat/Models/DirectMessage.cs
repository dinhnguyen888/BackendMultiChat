namespace BackendMultiChat.Models
{
    public class DirectMessage
    {
        public int DirectMessageId { get; set; }

        // allow null in condition that the account is deleted
        public Guid SenderId { get; set; }
        public Account Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public Account Receiver { get; set; }

        public string MessageText { get; set; }
        public DateTime SentDateTime { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}
