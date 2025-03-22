using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface IDirectMessageService
    {
        Task<DMGetDto> CreateDirectMessageAsync(DMPostDto dto);
        Task<bool> DeleteDirectMessageAsync(int id);
        Task<IEnumerable<DMGetDto>> GetAllDMAsync(Guid senderId, Guid receiverId);
    }
}