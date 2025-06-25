using chatApp.DTOs;
using chatApp.Models;
using ChatApp.Repositories.Interfaces;
using ChatApp.Services.Interfaces;

namespace ChatApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;

        public MessageService(IMessageRepository messageRepo)
        {
            _messageRepo = messageRepo;
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(Guid senderId,  Guid receiverId)
            => await _messageRepo.GetMessagesAsync(senderId, receiverId);

        public async Task<int> SendMessageAsync(Message message)
            => await _messageRepo.SendMessageAsync(message);

        public async Task MarkMessageAsReadAsync(Guid SenderID, Guid ReceiverID)
            => await _messageRepo.MarkAsReadAsync(SenderID,ReceiverID);

        public async Task<List<UnreadCountDto>> GetUnreadCountsForAllUsersAsync(Guid currentUserId)
            => await _messageRepo.GetUnreadCountsForAllUsersAsync(currentUserId);

        public Task<int> GetUnreadCountBetweenUsersAsync(Guid senderId, Guid receiverId)=> _messageRepo.GetUnreadCountBetweenUsersAsync(senderId, receiverId);

    }
}
