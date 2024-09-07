using BackendMultiChat.Data;
using BackendMultiChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BackendMultiChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.PhoneNumber) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Invalid login attempt.");
            }

            var user = _context.Contacts
                .SingleOrDefault(c => c.PhoneNumber == loginDto.PhoneNumber && c.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid phone number or password.");
            }
            return Ok(new
            {
                Message = "Login successful.",
                FullName = user.FullName,
                Id =user.ContactId,
                PhoneNumber = user.PhoneNumber,
                
            });
        }
    }
}
