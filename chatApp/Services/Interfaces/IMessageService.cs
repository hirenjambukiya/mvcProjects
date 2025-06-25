using System.Collections.Generic;
using System.Threading.Tasks;
using chatApp.DTOs;
using chatApp.Models;

namespace ChatApp.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetChatHistoryAsync(Guid senderId, Guid receiverId);
        Task<int> SendMessageAsync(Message message);
        Task MarkMessageAsReadAsync(Guid SenderID, Guid ReceiverID);
        Task<List<UnreadCountDto>> GetUnreadCountsForAllUsersAsync(Guid currentUserId);

        Task<int> GetUnreadCountBetweenUsersAsync(Guid senderId, Guid receiverId);

    }
}
