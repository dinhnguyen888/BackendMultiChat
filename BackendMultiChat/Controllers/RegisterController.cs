using BackendMultiChat.Data;
using BackendMultiChat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BackendMultiChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (registerDto == null || string.IsNullOrEmpty(registerDto.FullName) || string.IsNullOrEmpty(registerDto.PhoneNumber) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Invalid registration attempt.");
            }

            // Kiểm tra liên hệ có tồn tại không
            var existingUser = await _context.Contacts.SingleOrDefaultAsync(c => c.PhoneNumber == registerDto.PhoneNumber);
            if (existingUser != null)
            {
                return Conflict("Phone number already in use.");
            }

            // Tạo liên hệ mới
            var newUser = new Contact
            {
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                Password = registerDto.Password
            };

            _context.Contacts.Add(newUser);
            await _context.SaveChangesAsync(); // Lưu liên hệ mới

            // Lấy tất cả liên hệ cũ
            var existingContacts = await _context.Contacts.Where(c => c.ContactId != newUser.ContactId).ToListAsync();

            // Tạo conversation cho liên hệ mới với mỗi liên hệ cũ
            foreach (var contact in existingContacts)
            {
                // Tạo một conversation mới cho liên hệ mới với liên hệ cũ
                var newConversation = new Conversation
                {
                    ConversationName = null // Nếu không muốn đặt tên
                };
                _context.Conversations.Add(newConversation);
                await _context.SaveChangesAsync();

                // Thêm GroupMember cho liên hệ cũ trong conversation mới
                var groupMemberForExistingContact = new GroupMember
                {
                    ContactId = contact.ContactId,
                    ConversationId = newConversation.ConversationId // Conversation mới
                };
                _context.GroupMembers.Add(groupMemberForExistingContact);

                // Thêm GroupMember cho liên hệ mới trong conversation mới
                var groupMemberForNewUser = new GroupMember
                {
                    ContactId = newUser.ContactId,
                    ConversationId = newConversation.ConversationId // Conversation mới
                };
                _context.GroupMembers.Add(groupMemberForNewUser);

                await _context.SaveChangesAsync();
            }

            return Ok(new { Message = "Registration successful." });
        }


    }
}
