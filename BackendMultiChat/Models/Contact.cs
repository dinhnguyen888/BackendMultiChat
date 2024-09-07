namespace BackendMultiChat.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        // Navigation property
        public ICollection<GroupMember> GroupMembers { get; set; }
    }
}
