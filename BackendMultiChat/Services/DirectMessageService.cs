using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Hubs;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using Humanizer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Services
{
    public class DirectMessageService : IDirectMessageService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly IFileHandler _fileHandler;
        public DirectMessageService(AppDbContext context, IMapper mapper, IHubContext<PresenceHub> presenceHub, IFileHandler fileHandler)
        {
            _context = context;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _fileHandler = fileHandler;
        }

        public async Task<IEnumerable<DMGetDto>> GetAllDMAsync(Guid senderId, Guid receiverId)
        {
            var directMessages = await _context.DirectMessages
                .Where(dm => dm.SenderId == senderId && dm.ReceiverId == receiverId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DMGetDto>>(directMessages);
        }

        public async Task<DMGetDto> CreateDirectMessageAsync(DMPostDto dto)
        {
            var message = _mapper.Map<DirectMessage>(dto);
            _context.DirectMessages.Add(message);
            await _context.SaveChangesAsync();

            await _presenceHub.Clients.User(dto.ReceiverId.ToString()).SendAsync("ReceiveDirectMessage", _mapper.Map<DMGetDto>(message));
            return _mapper.Map<DMGetDto>(message);
        }

        public async Task<bool> DeleteDirectMessageAsync(int id)
        {
            var message = await _context.DirectMessages.FindAsync(id);
            if (message == null) return false;
            if (message.FileName != null)
            {
                // Delete file in server
                 _fileHandler.DeleteFileInServer(message.FileName);
            }
            _context.DirectMessages.Remove(message);

            // Save changes
            await _context.SaveChangesAsync();

            // Send notification to receiver
            await _presenceHub.Clients.User(message.ReceiverId.ToString())
                .SendAsync("DirectMessageDeleted", _mapper.Map<DMGetDto>(message));

            return true;
        }

    }
}
