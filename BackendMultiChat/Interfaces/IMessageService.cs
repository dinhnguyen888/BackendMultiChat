using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface IMessageService
    {
        Task<RMGetDto> CreateMessageAsync(RMPostDto dto);
        Task<bool> DeleteMessageAsync(int id);
        Task<IEnumerable<RMGetDto>> GetAllMessagesAsync(Guid roomId);
        Task<RMGetDto?> GetMessageByIdAsync(int id);
    }
}