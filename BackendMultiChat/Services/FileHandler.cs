using BackendMultiChat.Interfaces;

namespace BackendMultiChat.Services
{
    public class FileHandler : IFileHandler
    {
        public void DeleteFileInServer(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else throw new Exception("File not found");
        }
    }
}
