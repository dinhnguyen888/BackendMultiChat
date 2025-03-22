namespace BackendMultiChat.Dtos
{
    public class FileGetDto
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }

    public class FilePostToRoomDto
    {
        public IFormFile File { get; set; }
        public string SenderName { get; set; }
        public Guid RoomId { get; set; }
    }

    public class FilePostToDMDto
    {
        public IFormFile File { get; set; }
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
    }
}
