namespace BackendMultiChat.Interfaces
{

    // Get an error A circular dependency was detected for the service of type,
    // so IFileHander is created to handle the file deletion
    public interface IFileHandler
    {
        void DeleteFileInServer(string fileName);
    }
}
