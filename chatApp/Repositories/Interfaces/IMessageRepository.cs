using System.Collections.Generic;
using System.Threading.Tasks;
using chatApp.DTOs;
using chatApp.Models;

namespace ChatApp.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesAsync(Guid senderId, Guid receiverId);
        Task<int> SendMessageAsync(Message message);
        Task MarkAsReadAsync(Guid SenderID, Guid ReceiverID);
        Task<List<UnreadCountDto>> GetUnreadCountsForAllUsersAsync(Guid currentUserId);

        Task<int> GetUnreadCountBetweenUsersAsync(Guid senderId, Guid receiverId);

    }
}
