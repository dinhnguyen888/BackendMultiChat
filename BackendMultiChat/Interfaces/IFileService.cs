using BackendMultiChat.Dtos;
using System.Threading.Tasks;

public interface IFileService
{
    Task<bool> DeleteFileInRM(int fileId);

    Task<RMGetDto> SendFileToRM(FilePostToRoomDto dto);
    Task<DMGetDto> SendFileToDM(IFormFile file, Guid senderId, Guid receiverId);
    Task<IEnumerable<FileGetDto>> ViewFilesInRM(Guid roomId);
    Task<IEnumerable<FileGetDto>> ViewFileInDM(string token, Guid receiver);
}