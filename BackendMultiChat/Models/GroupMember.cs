using BackendMultiChat.Models;
using System;

namespace BackendMultiChat.Models
{
    public class GroupMember
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public int RoomId { get; set; }
        public Room Rooms { get; set; }

        public DateTime JoinedDateTime { get; set; }
        public DateTime? LeftDateTime { get; set; } // Nullable in case the user hasn't left the group
    }
}
