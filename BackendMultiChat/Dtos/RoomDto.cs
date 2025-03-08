namespace BackendMultiChat.Dtos
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public List<Guid> MemberIds { get; set; }
    }
}
