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

        private readonly IFileHandler _fileHander;
        public MessageService(AppDbContext context, IMapper mapper, IHubContext<PresenceHub> presenceHub, IFileHandler fileHander)
        {
            _context = context;
            _mapper = mapper;
            _presenceHub = presenceHub;
         
            _fileHander = fileHander;
        }

        public async Task<IEnumerable<RMGetDto>> GetAllMessagesAsync(Guid roomId)
        {
            var messages = await _context.RoomMessages
                .Where(m => m.RoomId == roomId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<RMGetDto>>(messages);
        }

        public async Task<RMGetDto> CreateMessageAsync(RMPostDto dto)
        {
            var message = _mapper.Map<RoomMessage>(dto);
            _context.RoomMessages.Add(message);
            await _context.SaveChangesAsync();

            await _presenceHub.Clients.Group(dto.RoomId.ToString()).SendAsync("ReceiveMessage", _mapper.Map<RMGetDto>(message));
            return _mapper.Map<RMGetDto>(message);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _context.RoomMessages.FindAsync(id);
            if (message == null) return false;
            if (message.FileName != null)
            {
                 _fileHander.DeleteFileInServer(message.FileName);
            }
            _context.RoomMessages.Remove(message);
            await _context.SaveChangesAsync();

            // Gửi thông báo cho các client trong cùng nhóm
            await _presenceHub.Clients.Group(message.RoomId.ToString())
                .SendAsync("MessageDeleted", id);

            return true;
        }


        public async Task<RMGetDto?> GetMessageByIdAsync(int id)
        {
            var message = await _context.RoomMessages.FindAsync(id);
            return message == null ? null : _mapper.Map<RMGetDto>(message);
        }
    }
}
