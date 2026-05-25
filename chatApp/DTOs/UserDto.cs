namespace ChatApp.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string LastActive { get; set; } = string.Empty;

        public int UnreadCount { get; set; } = 0;
    }
}
