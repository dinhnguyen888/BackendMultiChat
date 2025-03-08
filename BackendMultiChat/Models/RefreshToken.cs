namespace BackendMultiChat.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public Guid AccountId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryTime { get; set; } 
        public Account Account { get; set; }
    }
}
