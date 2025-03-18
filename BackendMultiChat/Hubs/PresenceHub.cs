using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BackendMultiChat.Hubs
{
    public class PresenceHub : Hub, IPresenceHub
    {
        // Save userId, userName and connections
        private static readonly Dictionary<string, (string userName, HashSet<string> connections)> _onlineContacts = new();

        public async Task OnConnectAsync(string userId, string userName)
        {
            bool isOnline = false;

            lock (_onlineContacts)
            {
                if (!_onlineContacts.TryGetValue(userId, out var userInfo))
                {
                    userInfo = (userName, new HashSet<string>());
                    _onlineContacts[userId] = userInfo;
                    isOnline = true;
                }

                _onlineContacts[userId].connections.Add(userId);
            }

            if (isOnline)
            {
                await Clients.All.SendAsync("UserConnected", userId, userName, true);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? userId = null;
            bool isOffline = false;

            lock (_onlineContacts)
            {
                // Tìm user theo AccountId (thay vì ConnectionId)
                foreach (var kvp in _onlineContacts)
                {
                    if (kvp.Value.connections.Remove(kvp.Key))
                    {
                        userId = kvp.Key;

                        if (kvp.Value.connections.Count == 0)
                        {
                            _onlineContacts.Remove(userId);
                            isOffline = true;
                        }
                        break;
                    }
                }
            }

            if (userId != null && isOffline)
            {
                var userName = _onlineContacts.TryGetValue(userId, out var userInfo) ? userInfo.userName : "";
                await Clients.All.SendAsync("UserDisconnected", userId, userName, false);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public Task<List<(string userId, string userName)>> ViewOnlineAsync()
        {
            var onlineUsers = _onlineContacts
                .Select(u => (u.Key, u.Value.userName))
                .ToList();

            return Task.FromResult(onlineUsers);
        }


        public async Task JoinRoom(Guid roomId)
        {
            string userId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(userId))
                return;

            // Thêm userId vào group để theo dõi chính xác người dùng trong phòng
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

            lock (_onlineContacts)
            {
                if (_onlineContacts.TryGetValue(userId, out var userInfo))
                {
                    userInfo.connections.Add(Context.ConnectionId);
                }
            }

            // Thông báo cho các user khác trong phòng là user này đã tham gia
            await Clients.Group(roomId.ToString()).SendAsync("UserJoined", userId);
        }


        // Leave room
        public async Task LeaveRoom(Guid roomId)
        {
            string userId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(userId))
                return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());

            lock (_onlineContacts)
            {
                if (_onlineContacts.TryGetValue(userId, out var userInfo))
                {
                    userInfo.connections.Remove(Context.ConnectionId);

                    // Nếu user không còn kết nối nào thì xóa khỏi danh sách online
                    if (userInfo.connections.Count == 0)
                    {
                        _onlineContacts.Remove(userId);
                    }
                }
            }

            // Thông báo cho các user khác trong phòng là user này đã rời đi
            await Clients.Group(roomId.ToString()).SendAsync("UserLeft", userId);
        }

    }
}
