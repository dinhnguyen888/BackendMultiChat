using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface IMessageService
    {
        Task<MessageGetDto> CreateMessageAsync(MessagePostDto dto);
        Task<bool> DeleteMessageAsync(int id);
        Task<IEnumerable<MessageGetDto>> GetAllMessagesAsync(Guid roomId);
        Task<MessageGetDto?> GetMessageByIdAsync(int id);
    }
}