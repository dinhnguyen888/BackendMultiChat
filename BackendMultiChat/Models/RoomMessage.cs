using System;

namespace BackendMultiChat.Models
{
    public class RoomMessage
    {
        public int RoomMessageId { get; set; }
        public string SenderName { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDateTime { get; set; } = DateTime.Now;
        public string? FileName { get; set; }  // Name of the file sent
        public string? FileUrl { get; set; }   // URL to access the file

        // Foreign key
        public Guid RoomId { get; set; }
        public Room Rooms { get; set; }
    }
}
