using System;

namespace BackendMultiChat.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string FromNumber { get; set; } // Assuming FromNumber is a phone number
        public string MessageText { get; set; }
        public DateTime SentDateTime { get; set; }
        public string? FileName { get; set; }  // Name of the file sent
        public string? FileUrl { get; set; }   // URL to access the file

        // Foreign key
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
