using BackendMultiChat.Dtos;

public interface IFileService
{
    Task<MessageGetDto> SendFile(FilePostDto dto);
    Task<IEnumerable<FileGetDto>> ViewFiles(Guid roomId);
    Task<bool> DeleteFile(int fileId);
}