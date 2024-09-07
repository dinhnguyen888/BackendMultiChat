using BackendMultiChat.Data;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;

public class StatusHub : Hub
{
    // Dictionary để lưu user online (ConnectionId -> PhoneNumber)
    private static ConcurrentDictionary<string, string> _onlineContacts = new ConcurrentDictionary<string, string>();

    private readonly AppDbContext _context;

    public StatusHub(AppDbContext context)
    {
        _context = context;
    }

    public Task RegisterContact(string userPhoneNumber)
    {
        if (!string.IsNullOrEmpty(userPhoneNumber))
        {
            // Thêm userPhoneNumber vào biến cục bộ với Context.ConnectionId làm key
            _onlineContacts.TryAdd(Context.ConnectionId, userPhoneNumber);

            // Gửi thông báo về người dùng mới kết nối tới tất cả các client
            return Clients.All.SendAsync("UserConnected", userPhoneNumber);
        }
        return Task.CompletedTask;
    }

    public override async Task OnDisconnectedAsync(System.Exception exception)
    {
        // Xóa người dùng khi ngắt kết nối
        if (_onlineContacts.TryRemove(Context.ConnectionId, out string userPhoneNumber))
        {
            // Gửi thông báo về người dùng ngắt kết nối tới tất cả các client
            await Clients.All.SendAsync("UserDisconnected", userPhoneNumber);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public Task<string[]> GetOnlineContacts()
    {
        // Trả về danh sách các user đang online từ biến cục bộ
        string[] userList = _onlineContacts.Values.ToArray();
        return Task.FromResult(userList);
    }

    // Gửi thông báo tới tất cả user online trong một conversation trừ người gửi
    public async Task SendNotificationToOnlineUsersInConversation(string fromNumber, string messageText, int conversationId)
    {
        // Lấy danh sách các thành viên trong conversation
        var conversationMembers = _context.GroupMembers
            .Where(gm => gm.ConversationId == conversationId)
            .Select(gm => gm.Contact)
            .ToList();

        foreach (var member in conversationMembers)
        {
            // Chỉ gửi thông báo tới user online và khác người gửi
            if (member.PhoneNumber != fromNumber && _onlineContacts.Values.Contains(member.PhoneNumber))
            {
                // Tìm connectionId của user online
                var connectionId = _onlineContacts.FirstOrDefault(x => x.Value == member.PhoneNumber).Key;

                if (!string.IsNullOrEmpty(connectionId))
                {
                    // Gửi thông báo qua ConnectionId
                    await Clients.Client(connectionId).SendAsync("ReceiveNotification", $"Tin nhắn mới: {messageText}");
                }
            }
        }
    }
}
