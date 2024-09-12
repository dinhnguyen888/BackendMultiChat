using BackendMultiChat.Data;
using BackendMultiChat.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        //get all account
        [HttpGet("get-all-account")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccount()
        {
            var contacts = await _context.Contacts.ToListAsync();

            return Ok(contacts);
        }

        //login admin panel
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.PhoneNumber) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Tài khoản không hơp lệ.");
            }

            var user = _context.Contacts
                .SingleOrDefault(c => c.PhoneNumber == loginDto.PhoneNumber && c.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Tài khoản hoặc mật khẩu sai.");
            }
            if(!user.IsAdmin)
            {
                return Unauthorized("Bạn không phải là admin");
            }
            
            return Ok(new
            {
                Message = "Login successful.",
                FullName = user.FullName,
                Id = user.ContactId,
                PhoneNumber = user.PhoneNumber,

            });
        }

        [HttpPost("change-admin-permission/{id}")]
        public async Task<IActionResult> UpdateAccount(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            // Ensure the IsAdmin field is not null before toggling
            if (contact.IsAdmin == null)
            {
                return BadRequest("Không thể thay đổi quyền admin do trạng thái admin hiện tại không xác định.");
            }

            // Toggle the IsAdmin property
            contact.IsAdmin = !contact.IsAdmin;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Log the exception if needed
                if (!ContactExists(id))
                {
                    return NotFound("Tài khoản không tồn tại.");
                }
                else
                {
                    return StatusCode(500, "Lỗi trong quá trình cập nhật. Vui lòng thử lại sau.");
                }
            }

            return Ok($"Cấp quyền thành công. Quyền admin đã được {(contact.IsAdmin == true ? "cấp" : "hủy")}.");
        }



        [HttpPut("update-account/{id}")]
        public async Task<IActionResult> UpdateAccount([FromRoute] int id, [FromBody] ContactDTO updatedContact)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound("Tài khoản không tồn tại");
            }

            // Kiểm tra nếu số điện thoại đã tồn tại trong hệ thống và không thuộc về tài khoản hiện tại
            var existingContactWithPhone = await _context.Contacts
                .FirstOrDefaultAsync(c => c.PhoneNumber == updatedContact.PhoneNumber && c.ContactId != id);

            if (existingContactWithPhone != null)
            {
                return Conflict("Số điện thoại đã tồn tại trên hệ thống.");
            }

            // Cập nhật thông tin tài khoản
            contact.FullName = updatedContact.FullName;
            contact.PhoneNumber = updatedContact.PhoneNumber;
            contact.Password = updatedContact.Password;

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Cập nhật thành công");
        }


        //delete account
        [HttpDelete("delete-account/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok("Đã xóa thành công");
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ContactId == id);
        }

        //// method to handle room
        // get all room
        [HttpGet("get-all-room")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllRooms()
        {
            var rooms = await _context.Conversations
                .Include(c => c.GroupMembers)
                .ThenInclude(gm => gm.Contact)
                .Where(c => c.ConversationName != null) // Loại bỏ phòng có tên null
                .Select(c => new
                {
                    RoomId = c.ConversationId,
                    RoomName = c.ConversationName,
                    Quantity = c.GroupMembers.Count, // Số lượng thành viên
                    Participants = c.GroupMembers.Select(gm => gm.Contact.FullName), // Danh sách thành viên
                    MemberId = c.GroupMembers.Select(gm => gm.Contact.ContactId)
                })
                .ToListAsync();

            return Ok(rooms);
        }

        [HttpPost("add-room")]
        public async Task<ActionResult> AddRoom([FromBody] ConversationDTO request)
        {
            // Kiểm tra xem tên phòng có hợp lệ không
            if (string.IsNullOrWhiteSpace(request.ConversationName))
            {
                return BadRequest("Tên phòng không được bỏ trống.");
            }

            // Tạo mới một phòng
            var newConversation = new Conversation
            {
                ConversationName = request.ConversationName,
                GroupMembers = new List<GroupMember>() // Khởi tạo danh sách GroupMembers trống
            };

            // Thêm các thành viên vào nhóm nếu có
            if (request.MemberIds != null)
            {
                foreach (var memberId in request.MemberIds)
                {
                    var contactExists = await _context.Contacts.AnyAsync(c => c.ContactId == memberId);
                    if (!contactExists)
                    {
                        return BadRequest($"Tài khoản với ID '{memberId}' không tồn tại.");
                    }

                    var newMember = new GroupMember
                    {
                        ContactId = memberId,
                        Conversation = newConversation,
                        JoinedDateTime = DateTime.Now
                    };
                    newConversation.GroupMembers.Add(newMember);
                }
            }

            // Lưu phòng vào database
            _context.Conversations.Add(newConversation);
            await _context.SaveChangesAsync();

            return Ok("Phòng đã được tạo thành công.");
        }

        [HttpDelete("delete-room/{roomId}")]
        public async Task<ActionResult> DeleteRoom(int roomId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.GroupMembers)
                .FirstOrDefaultAsync(c => c.ConversationId == roomId);

            if (conversation == null)
            {
                return NotFound("Phòng không tồn tại.");
            }

            // Xóa tất cả các thành viên trong nhóm trước khi xóa phòng
            _context.GroupMembers.RemoveRange(conversation.GroupMembers);

            // Xóa phòng
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();

            return Ok("Phòng đã được xóa thành công.");
        }



        [HttpPut("edit-room")]
        public async Task<ActionResult> EditRoom([FromBody] ConversationDTO request)
        {
            // Find the conversation by ID
            var conversation = await _context.Conversations
                .Include(c => c.GroupMembers)
                .FirstOrDefaultAsync(c => c.ConversationId == request.ConversationId);

            if (conversation == null)
            {
                return NotFound("Phòng không tồn tại");
            }

            // Update the conversation name if provided
            if (!string.IsNullOrWhiteSpace(request.ConversationName))
            {
                conversation.ConversationName = request.ConversationName;
            }

            // Update group members if the MemberIds list is provided
            if (request.MemberIds != null)
            {
                // Find existing members in the database
                var existingMembers = conversation.GroupMembers.ToList();

                // Remove members that are not in the new list of MemberIds
                foreach (var member in existingMembers)
                {
                    if (!request.MemberIds.Contains(member.ContactId))
                    {
                        _context.GroupMembers.Remove(member);
                    }
                }

                // Add new members from the provided list if they are not already in the group
                foreach (var memberId in request.MemberIds)
                {
                    // Check if the member already exists in the group
                    if (!existingMembers.Any(m => m.ContactId == memberId))
                    {
                        // Check if the Contact exists in the Contacts table
                        var contactExists = await _context.Contacts.AnyAsync(c => c.ContactId == memberId);

                        if (!contactExists)
                        {
                            // Return a bad request response if the ContactId does not exist
                            return BadRequest($"tài khoản với ID '{memberId}' không tồn tại.");
                        }

                        // Add the new member to the group
                        var newMember = new GroupMember
                        {
                            ContactId = memberId,
                            ConversationId = conversation.ConversationId,
                            JoinedDateTime = DateTime.Now // Set joined date as now
                        };
                        _context.GroupMembers.Add(newMember);
                    }
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok("cập nhật thành công");
        }


    }
}
