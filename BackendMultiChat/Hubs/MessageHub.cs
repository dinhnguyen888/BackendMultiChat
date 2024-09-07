using BackendMultiChat.Data;
using BackendMultiChat.Hubs;
using BackendMultiChat.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public class MessageHub : Hub
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _notificationHubContext;

    public MessageHub(AppDbContext context, IHubContext<NotificationHub> notificationHubContext)
    {
        _context = context;
        _notificationHubContext = notificationHubContext;
    }

    public async Task SendMessage(string fromNumber, string messageText, int conversationId)
    {
        var sendTime = DateTime.Now;
        // Lưu tin nhắn vào database
        var message = new Message
        {
            FromNumber = fromNumber,
            MessageText = messageText,
            SentDateTime = sendTime,
            ConversationId = conversationId
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Gửi tin nhắn đến tất cả người dùng trong conversation
        await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", fromNumber, messageText, sendTime);
    
        
    }

    public async Task JoinConversation(int conversationId)
    {
        // Thêm người dùng vào nhóm
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

        // Lấy các tin nhắn cũ trong cuộc trò chuyện
        var messages = _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentDateTime)
            .ToList();

        // Gửi các tin nhắn cũ đến người dùng vừa tham gia
        foreach (var message in messages)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", message.FromNumber, message.MessageText, message.SentDateTime);
        }
    }
   


}
