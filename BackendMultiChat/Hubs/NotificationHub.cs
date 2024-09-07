using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace BackendMultiChat.Hubs
{
    public class NotificationHub : Hub
    {
        // Phương thức để gửi thông báo
        public async Task SendNotification(string userId, string message)
        {
            // Gửi thông báo đến client cụ thể (theo userId)
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }

}
