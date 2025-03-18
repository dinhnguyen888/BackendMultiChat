namespace BackendMultiChat.Dtos
{
    public class MessagePostDto
    {
        public string SenderName { get; set; }
        public string? MessageText { get; set; } // allow null for send file only
        public Guid RoomId { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}
