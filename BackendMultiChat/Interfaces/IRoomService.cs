using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface IRoomService
    {
        Task<bool> CreateNewRoom(RoomCreateDto dto);
        Task<bool> DeleteRoom(Guid roomId);
        Task<List<RoomGetDto>> GetAllRooms();
        Task<object> GetConversationsByPhoneNumber(string phoneNumber);
        Task<object> GetRoomByPhoneNumber(string phoneNumber);
        Task<IEnumerable<RoomGetDto>> GetRoomByUserId(Guid userId);
    }
}