using static BackendMultiChat.Models.Account;

namespace BackendMultiChat.Dtos
{
    public class AccountGetDto
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; }
    }

    public class AccountPostDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public Roles Role { get; set; }
    }

    public class AccountUpdateDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public Roles Role { get; set; }
    }

    public class AccountViewOnlineDto
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }       
        public string Role { get; set; }
        public bool IsOnline { get; set; }


    }
}
