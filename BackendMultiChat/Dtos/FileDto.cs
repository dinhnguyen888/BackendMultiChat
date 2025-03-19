namespace BackendMultiChat.Dtos
{
    public class FileGetDto
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }

    public class FilePostDto
    {
        public IFormFile File { get; set; }
        public string SenderName { get; set; }
        public Guid RoomId { get; set; }
    }
}
