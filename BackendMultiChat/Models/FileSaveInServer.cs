namespace BackendMultiChat.Models
{
    public class FileSaveInServer
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public int ConversationID { get; set; }
        public Conversation Conversation { get; set; }
    }
}
