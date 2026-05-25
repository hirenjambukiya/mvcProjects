namespace ChatApp.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string SentAt { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool IsSentByCurrentUser { get; set; }
    }
}
