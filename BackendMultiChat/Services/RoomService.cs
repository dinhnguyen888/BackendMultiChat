using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Services
{
    public class RoomService
    {
        private readonly AppDbContext _context;
        public RoomService(AppDbContext context)
        {
            _context = context;
        }

     
        public async Task<object> GetAllRooms()
        {
            var rooms = await _context.Rooms
                .Include(c => c.GroupMembers)
                .ThenInclude(gm => gm.Account)
                .Where(c => c.RoomName != null) // Loại bỏ phòng có tên null
                .Select(c => new
                {
                    RoomId = c.RoomId,
                    RoomName = c.RoomName,
                    Quantity = c.GroupMembers.Count, // Số lượng thành viên
                    Participants = c.GroupMembers.Select(gm => gm.Account.FullName), // Danh sách thành viên
                    MemberId = c.GroupMembers.Select(gm => gm.Account.AccountId)
                })
                .ToListAsync();

            return rooms;
        }

        public async Task<bool> AddRoom( RoomDto request)
        {
            // Kiểm tra xem tên phòng có hợp lệ không
            if (string.IsNullOrWhiteSpace(request.RoomName))
            {
                return false;
            }

            // Tạo mới một phòng
            var newConversation = new Room
            {
                RoomName = request.RoomName,
                GroupMembers = new List<GroupMember>() // Khởi tạo danh sách GroupMembers trống
            };

            // Thêm các thành viên vào nhóm nếu có
            if (request.MemberIds != null)
            {
                foreach (var memberId in request.MemberIds)
                {
                    var contactExists = await _context.Accounts.AnyAsync(c => c.AccountId == memberId);
                    if (!contactExists)
                    {
                        return false;
                    }

                    var newMember = new GroupMember
                    {
                        AccountId = memberId,
                        Rooms = newConversation,
                        JoinedDateTime = DateTime.Now
                    };
                    newConversation.GroupMembers.Add(newMember);
                }
            }

            // Lưu phòng vào database
            _context.Rooms.Add(newConversation);
            await _context.SaveChangesAsync();

            return false;
        }

        public async Task<bool> DeleteRoom(int roomId)
        {
            var conversation = await _context.Rooms
                .Include(c => c.GroupMembers)
                .FirstOrDefaultAsync(c => c.RoomId == roomId);

            if (conversation == null)
            {
                return false ;
            }

            // Xóa tất cả các thành viên trong nhóm trước khi xóa phòng
            _context.GroupMembers.RemoveRange(conversation.GroupMembers);

            // Xóa phòng
            _context.Rooms.Remove(conversation);
            await _context.SaveChangesAsync();

            return false;
        }



    
        public async Task<bool> EditRoom([FromBody] RoomDto request)
        {
            // Find the conversation by ID
            var conversation = await _context.Rooms
                .Include(c => c.GroupMembers)
                .FirstOrDefaultAsync(c => c.RoomId == request.RoomId);

            if (conversation == null)
            {
                return false;
            }

            // Update the conversation name if provided
            if (!string.IsNullOrWhiteSpace(request.RoomName))
            {
                conversation.RoomName = request.RoomName;
            }

            // Update group members if the MemberIds list is provided
            if (request.MemberIds != null)
            {
                // Find existing members in the database
                var existingMembers = conversation.GroupMembers.ToList();

                // Remove members that are not in the new list of MemberIds
                foreach (var member in existingMembers)
                {
                    if (!request.MemberIds.Contains(member.AccountId))
                    {
                        _context.GroupMembers.Remove(member);
                    }
                }

                // Add new members from the provided list if they are not already in the group
                foreach (var memberId in request.MemberIds)
                {
                    // Check if the member already exists in the group
                    if (!existingMembers.Any(m => m.AccountId == memberId))
                    {
                        // Check if the Contact exists in the Contacts table
                        var contactExists = await _context.Accounts.AnyAsync(c => c.AccountId == memberId);

                        if (!contactExists)
                        {
                            // Return a bad request response if the ContactId does not exist
                            return false;
                        }

                        // Add the new member to the group
                        var newMember = new GroupMember
                        {
                            AccountId = memberId,
                            RoomId = conversation.RoomId,
                            JoinedDateTime = DateTime.Now // Set joined date as now
                        };
                        _context.GroupMembers.Add(newMember);
                    }
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true    ;
        }

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
