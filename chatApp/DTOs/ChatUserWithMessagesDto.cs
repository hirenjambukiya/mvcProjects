using System.Collections.Generic;

namespace ChatApp.DTOs
{
    public class ChatUserWithMessagesDto
    {
        public UserDto ChatPartner { get; set; } = new();
        public List<MessageDto> Messages { get; set; } = new();
    }
}
