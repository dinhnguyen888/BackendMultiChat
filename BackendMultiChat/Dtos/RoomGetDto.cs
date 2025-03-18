namespace BackendMultiChat.Dtos
{
    public class RoomGetDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public int MemberCount { get; set; }
        public int OnlineUserCount { get; set; }
    }
}
