
using static BackendMultiChat.Models.Account;

namespace BackendMultiChat.Dtos
{
    public class AccountGetDto
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public Roles Role { get; set; }
    }
}
