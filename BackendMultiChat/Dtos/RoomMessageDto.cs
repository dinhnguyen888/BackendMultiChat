namespace BackendMultiChat.Dtos
{
    public class RMGetDto
    {
        public int RoomMessageId { get; set; }
        public string SenderName { get; set; }
        public string MessageText { get; set; }
        public Guid RoomId { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }


    }

    public class RMPostDto
    {
        public string SenderName { get; set; }
        public string MessageText { get; set; } 
        public Guid RoomId { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}
