using BackendMultiChat.Models;
using System;

namespace BackendMultiChat.Models
{
    public class GroupMember
    {
        public int ContactId { get; set; }
        public Contact Contact { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public DateTime JoinedDateTime { get; set; }
        public DateTime? LeftDateTime { get; set; } // Nullable in case the user hasn't left the group
    }
}
