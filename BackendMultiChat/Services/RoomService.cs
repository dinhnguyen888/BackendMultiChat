using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Hubs;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPresenceHub _presenceHub;
        public RoomService(AppDbContext context, IMapper mapper, IPresenceHub hubContext)
        {
            _context = context;
            _mapper = mapper;
            _presenceHub = hubContext;
        }


        public async Task<List<RoomGetDto>> GetAllRooms()
        {
            // Get online users list
            var onlineUsers = await _presenceHub.ViewOnlineAsync();
            var onlineUserIds = onlineUsers.Select(u => u.userId).ToHashSet();

            var rooms = await _context.Rooms
                .Include(c => c.GroupMembers)
                .ThenInclude(gm => gm.Account)
                .Select(c => new RoomGetDto
                {
                    RoomId = c.RoomId,
                    RoomName = c.RoomName,
                    MemberCount = c.GroupMembers.Count,
                    OnlineUserCount = c.GroupMembers
                        .Count(gm => onlineUserIds.Contains(gm.Account.AccountId.ToString()))
                })
                .ToListAsync();

            return rooms;
        }


        public async Task<bool> CreateNewRoom(RoomCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoomName))
            {
                throw new ArgumentNullException(nameof(dto.RoomName));
            }
            
            // Generate a new RoomId for the new room
            dto.RoomId = Guid.NewGuid();

            // Map from dto to Model
            var mappedRoom = _mapper.Map<Room>(dto);
            
            // Set RoomId cho từng GroupMember
            if (mappedRoom.GroupMembers != null)
            {
                foreach (var member in mappedRoom.GroupMembers)
                {
                    member.RoomId = mappedRoom.RoomId;
                }
            }

            _context.Rooms.Add(mappedRoom);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteRoom(Guid roomId)
        {
            var room = await _context.Rooms.Include(r => r.GroupMembers).Include(r => r.Messages).SingleOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                return false;
            }
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return true;
        }

        // Get room that has a userId in the group members, with MemberCount and OnlineUserCount
        public async Task<IEnumerable<RoomGetDto>> GetRoomByUserId(Guid userId)
        {
            // Get online users list
            var onlineUsers = await _presenceHub.ViewOnlineAsync();
            var onlineUserIds = onlineUsers.Select(u => u.userId).ToHashSet();

            var rooms = await _context.GroupMembers
                .Include(gm => gm.Rooms)
                .ThenInclude(r => r.GroupMembers)
                .Where(gm => gm.AccountId == userId)
                .Select(gm => new RoomGetDto
                {
                    RoomId = gm.Rooms.RoomId,
                    RoomName = gm.Rooms.RoomName,
                    MemberCount = gm.Rooms.GroupMembers.Count,
                    OnlineUserCount = gm.Rooms.GroupMembers
                        .Count(member => onlineUserIds.Contains(member.Account.AccountId.ToString()))
                })
                .ToListAsync();

            return rooms ?? new List<RoomGetDto>();
        }






        //public async Task<bool> EditRoom( RoomDto request)
        //{
        //    // Find the conversation by ID
        //    var conversation = await _context.Rooms
        //        .Include(c => c.GroupMembers)
        //        .FirstOrDefaultAsync(c => c.RoomId == request.RoomId);

        //    if (conversation == null)
        //    {
        //        return false;
        //    }

        //    // Update the conversation name if provided
        //    if (!string.IsNullOrWhiteSpace(request.RoomName))
        //    {
        //        conversation.RoomName = request.RoomName;
        //    }

        //    // Update group members if the MemberIds list is provided
        //    if (request.MemberIds != null)
        //    {
        //        // Find existing members in the database
        //        var existingMembers = conversation.GroupMembers.ToList();

        //        // Remove members that are not in the new list of MemberIds
        //        foreach (var member in existingMembers)
        //        {
        //            if (!request.MemberIds.Contains(member.AccountId))
        //            {
        //                _context.GroupMembers.Remove(member);
        //            }
        //        }

        //        // Add new members from the provided list if they are not already in the group
        //        foreach (var memberId in request.MemberIds)
        //        {
        //            // Check if the member already exists in the group
        //            if (!existingMembers.Any(m => m.AccountId == memberId))
        //            {
        //                // Check if the Contact exists in the Contacts table
        //                var contactExists = await _context.Accounts.AnyAsync(c => c.AccountId == memberId);

        //                if (!contactExists)
        //                {
        //                    // Return a bad request response if the ContactId does not exist
        //                    return false;
        //                }

        //                // Add the new member to the group
        //                var newMember = new GroupMember
        //                {
        //                    AccountId = memberId,
        //                    RoomId = conversation.RoomId,
        //                    JoinedDateTime = DateTime.Now // Set joined date as now
        //                };
        //                _context.GroupMembers.Add(newMember);
        //            }
        //        }
        //    }

        //    // Save changes to the database
        //    await _context.SaveChangesAsync();

        //    return true    ;
        //}    //public async Task<bool> EditRoom( RoomDto request)
        //{
        //    // Find the conversation by ID
        //    var conversation = await _context.Rooms
        //        .Include(c => c.GroupMembers)
        //        .FirstOrDefaultAsync(c => c.RoomId == request.RoomId);

        //    if (conversation == null)
        //    {
        //        return false;
        //    }

        //    // Update the conversation name if provided
        //    if (!string.IsNullOrWhiteSpace(request.RoomName))
        //    {
        //        conversation.RoomName = request.RoomName;
        //    }

        //    // Update group members if the MemberIds list is provided
        //    if (request.MemberIds != null)
        //    {
        //        // Find existing members in the database
        //        var existingMembers = conversation.GroupMembers.ToList();

        //        // Remove members that are not in the new list of MemberIds
        //        foreach (var member in existingMembers)
        //        {
        //            if (!request.MemberIds.Contains(member.AccountId))
        //            {
        //                _context.GroupMembers.Remove(member);
        //            }
        //        }

        //        // Add new members from the provided list if they are not already in the group
        //        foreach (var memberId in request.MemberIds)
        //        {
        //            // Check if the member already exists in the group
        //            if (!existingMembers.Any(m => m.AccountId == memberId))
        //            {
        //                // Check if the Contact exists in the Contacts table
        //                var contactExists = await _context.Accounts.AnyAsync(c => c.AccountId == memberId);

        //                if (!contactExists)
        //                {
        //                    // Return a bad request response if the ContactId does not exist
        //                    return false;
        //                }

        //                // Add the new member to the group
        //                var newMember = new GroupMember
        //                {
        //                    AccountId = memberId,
        //                    RoomId = conversation.RoomId,
        //                    JoinedDateTime = DateTime.Now // Set joined date as now
        //                };
        //                _context.GroupMembers.Add(newMember);
        //            }
        //        }
        //    }

        //    // Save changes to the database
        //    await _context.SaveChangesAsync();

        //    return true    ;
        //}

        public async Task<object> GetConversationsByPhoneNumber(string phoneNumber)
        {
            // Truy xuất tất cả các liên hệ trong các cuộc trò chuyện nơi liên hệ có số điện thoại trùng khớp và cuộc trò chuyện không có ConversationName
            var result = await _context.GroupMembers
                .Include(gm => gm.Rooms)
                .Include(gm => gm.Account)
                .Where(gm => gm.Account.PhoneNumber == phoneNumber && gm.Rooms.RoomName == null)
                .SelectMany(gm => _context.GroupMembers
                    .Where(gm2 => gm2.RoomId == gm.RoomId && gm2.Account.PhoneNumber != phoneNumber)
                    .Select(gm2 => new
                    {
                        gm2.RoomId,
                        ContactName = gm2.Account.FullName,
                        PhoneNumber = gm2.Account.PhoneNumber
                    })
                )
                .ToListAsync();

            return result;
        }


        public async Task<object> GetRoomByPhoneNumber(string phoneNumber)
        {
            // Lấy tất cả các phòng mà liên hệ với số điện thoại này tham gia, nhưng chỉ lấy các phòng có tên cuộc trò chuyện (ConversationName không phải null)
            var result = await _context.GroupMembers
                .Include(gm => gm.Rooms) // Bao gồm dữ liệu cuộc trò chuyện
                .Where(gm => gm.Account.PhoneNumber == phoneNumber && gm.Rooms.RoomName != null) // Lọc theo số điện thoại và chỉ lấy các phòng có tên
                .Select(gm => new
                {
                    gm.RoomId, // Lấy Id của cuộc trò chuyện
                    ContactName = gm.Rooms.RoomName, // Lấy tên của phòng (ConversationName)
                    PhoneNumber = "NaN" // PhoneNumber không áp dụng trong trường hợp này
                })
                .Distinct() // Đảm bảo không có các phòng trùng lặp
                .ToListAsync();

            return result;
        }

    }
}
