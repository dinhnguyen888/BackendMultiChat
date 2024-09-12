using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendMultiChat.Data;
using BackendMultiChat.Models;

namespace BackendMultiChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Contacts
      


        [HttpGet("get-all-contacts/{phoneNumber}")]
        public async Task<ActionResult<IEnumerable<object>>> GetConversationsByPhoneNumber(string phoneNumber)
        {
            // Truy xuất tất cả các liên hệ trong các cuộc trò chuyện nơi liên hệ có số điện thoại trùng khớp và cuộc trò chuyện không có ConversationName
            var result = await _context.GroupMembers
                .Include(gm => gm.Conversation)
                .Include(gm => gm.Contact)
                .Where(gm => gm.Contact.PhoneNumber == phoneNumber && gm.Conversation.ConversationName == null)
                .SelectMany(gm => _context.GroupMembers
                    .Where(gm2 => gm2.ConversationId == gm.ConversationId && gm2.Contact.PhoneNumber != phoneNumber)
                    .Select(gm2 => new
                    {
                        gm2.ConversationId,
                        ContactName = gm2.Contact.FullName,
                        PhoneNumber = gm2.Contact.PhoneNumber
                    })
                )
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("get-all-room/{phoneNumber}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoomByPhoneNumber(string phoneNumber)
        {
            // Lấy tất cả các phòng mà liên hệ với số điện thoại này tham gia, nhưng chỉ lấy các phòng có tên cuộc trò chuyện (ConversationName không phải null)
            var result = await _context.GroupMembers
                .Include(gm => gm.Conversation) // Bao gồm dữ liệu cuộc trò chuyện
                .Where(gm => gm.Contact.PhoneNumber == phoneNumber && gm.Conversation.ConversationName != null) // Lọc theo số điện thoại và chỉ lấy các phòng có tên
                .Select(gm => new
                {
                    gm.ConversationId, // Lấy Id của cuộc trò chuyện
                    ContactName = gm.Conversation.ConversationName, // Lấy tên của phòng (ConversationName)
                    PhoneNumber = "NaN" // PhoneNumber không áp dụng trong trường hợp này
                })
                .Distinct() // Đảm bảo không có các phòng trùng lặp
                .ToListAsync();

            return Ok(result);
        }


    }
}
