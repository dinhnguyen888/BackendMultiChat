namespace BackendMultiChat.Dtos
{
    public class ProfileGetDto
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
