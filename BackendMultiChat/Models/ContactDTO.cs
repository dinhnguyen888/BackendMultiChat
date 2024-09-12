using System.Text.Json.Serialization;

namespace BackendMultiChat.Models
{
    public class ContactDTO
    {
        [JsonIgnore]
        public int ContactId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
