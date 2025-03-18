namespace BackendMultiChat.Models
{
    public class FileStorage
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public Guid RoomId { get; set; }
        public Room Rooms { get; set; }
    }
}
