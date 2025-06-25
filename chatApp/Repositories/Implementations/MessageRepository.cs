using System.Data;
using chatApp.DTOs;
using chatApp.Models;
using ChatApp.Repositories.Interfaces;
using Dapper;

namespace ChatApp.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IDbConnection _db;
        public MessageRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(Guid senderId, Guid receiverId)
        {
            var parameters = new { SenderId = senderId, ReceiverId = receiverId };
            return await _db.QueryAsync<Message>("USP_GetMessages", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> SendMessageAsync(Message message)
        {
            try
            {
                var parameters = new
                {
                    message.SenderId,
                    message.ReceiverId,
                    message.Content,
                    message.Timestamp
                };
                return await _db.ExecuteScalarAsync<int>("USP_SendMessage", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while sending the message.", ex);
            }
        }

        public async Task MarkAsReadAsync(Guid SenderID, Guid ReceiverID)
        {
            var parameters = new
            {
                senderId = SenderID,
                receiverId = ReceiverID
            };
            await _db.ExecuteAsync("USP_MarkMessageAsRead", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<UnreadCountDto>> GetUnreadCountsForAllUsersAsync(Guid currentUserId)
        {

            var parameters = new DynamicParameters();
            parameters.Add("@SenderId", currentUserId);

            var result = await _db.QueryAsync<UnreadCountDto>(
                "USP_GetCntMessageAsUnread",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<int> GetUnreadCountBetweenUsersAsync(Guid senderId, Guid receiverId)
        {
            try
            {
                var parameters = new { SenderId = senderId, ReceiverId = receiverId };

                return await _db.ExecuteScalarAsync<int>("GetUnreadCountBetweenUsers", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
