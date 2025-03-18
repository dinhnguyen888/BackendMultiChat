using BackendMultiChat.Models;

namespace BackendMultiChat.Dtos
{
    public class MessageGetDto
    {
        public int MessageId { get; set; }
        public string SenderName { get; set; }
        public string MessageText { get; set; }
        public Guid RoomId { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }


    }
}
