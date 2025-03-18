namespace BackendMultiChat.Interfaces
{
    public interface IPresenceHub
    {
        Task OnConnectAsync(string userId, string userName);
        Task OnDisconnectedAsync(Exception? exception);
        Task<List<(string userId, string userName)>> ViewOnlineAsync();
        Task JoinRoom(Guid roomId);
        Task LeaveRoom(Guid roomId);

    }
}