using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Hubs;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        public MessageService(AppDbContext context, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _context = context;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public async Task<IEnumerable<MessageGetDto>> GetAllMessagesAsync(Guid roomId)
        {
            var messages = await _context.Messages
                .Where(m => m.RoomId == roomId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<MessageGetDto>>(messages);
        }

        public async Task<MessageGetDto> CreateMessageAsync(MessagePostDto dto)
        {
            var message = _mapper.Map<Message>(dto);
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await _presenceHub.Clients.Group(dto.RoomId.ToString()).SendAsync("ReceiveMessage", _mapper.Map<MessageGetDto>(message));
            return _mapper.Map<MessageGetDto>(message);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null) return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            // Gửi thông báo cho các client trong cùng nhóm
            await _presenceHub.Clients.Group(message.RoomId.ToString())
                .SendAsync("MessageDeleted", id);

            return true;
        }


        public async Task<MessageGetDto?> GetMessageByIdAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            return message == null ? null : _mapper.Map<MessageGetDto>(message);
        }
    }
}
