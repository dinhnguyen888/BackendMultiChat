namespace BackendMultiChat.Dtos
{
    public class FilePostDto
    {
        public IFormFile File { get; set; }
        public string SenderName { get; set; }
        public Guid RoomId { get; set; }
    }
}
