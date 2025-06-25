using chatApp.Models;
using ChatApp.Services;
using ChatApp.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConnectionService _connectionService;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public ChatHub(IConnectionService connectionService, IUserService userService, IMessageService messageService)
        {
            _connectionService = connectionService;
            _userService = userService;
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {

                var username = Context.User.Identity?.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _userService.GetUserByNameAsync(username);
                    if (user != null)
                    {
                        await _connectionService.AddOrUpdateConnectionAsync(user.UserId, Context.ConnectionId);
                        await _userService.UpdateOnlineStatusAsync(user.UserId, true);
                    }
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try { 
            var username = Context.User.Identity?.Name;

            if (!string.IsNullOrEmpty(username))
            {
                var user = await _userService.GetUserByNameAsync(username);
                if (user != null)
                {
                    await _connectionService.RemoveConnectionAsync(user.UserId);
                    await _userService.UpdateOnlineStatusAsync(user.UserId, false);
                }
            }

            await base.OnDisconnectedAsync(exception);
            }

            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error in OnDisconnectedAsync: {ex.Message}");
            }
        }

        //public async Task SendMessage(string receiverUsername, string message)
        //{
        //    var senderUsername = Context.User.Identity?.Name;
        //    if (string.IsNullOrEmpty(senderUsername)) return;

        //    var sender = await _userService.GetByUsernameAsync(senderUsername);
        //    var receiver = await _userService.GetByUsernameAsync(receiverUsername);
        //    if (sender == null || receiver == null) return;

        //    var senderConnection = await _connectionService.GetByUserIdAsync(sender.Id);
        //    var receiverConnection = await _connectionService.GetByUserIdAsync(receiver.Id);

        //    // Save to database via MessageService (optional here, can be done in controller)
        //    // await _messageService.SendMessageAsync(sender.Id, receiver.Id, message);

        //    if (!string.IsNullOrEmpty(receiverConnection?.ConnectionId))
        //    {
        //        await Clients.Client(receiverConnection.ConnectionId).SendAsync("ReceiveMessage", senderUsername, message);
        //    }

        //    // Echo back to sender
        //    if (!string.IsNullOrEmpty(senderConnection?.ConnectionId))
        //    {
        //        await Clients.Client(senderConnection.ConnectionId).SendAsync("MessageSent", receiverUsername, message);
        //    }
        //}

        public async Task SendMessage(string senderUsername, string receiverUsername, string messageText)
        {
            
            var sender = await _userService.GetUserByNameAsync(senderUsername);
            var receiver = await _userService.GetUserByNameAsync(receiverUsername);

            var message = new Message
            {
                SenderId = sender.UserId,
                ReceiverId = receiver.UserId,
                Content = messageText,
                Timestamp = DateTime.Now
            };

            await _messageService.SendMessageAsync(message);
            
            // Send to receiver
            await Clients.All.SendAsync("ReceiveMessage", sender.UserId, messageText, DateTime.Now);

            int unreadCount = await _messageService.GetUnreadCountBetweenUsersAsync(receiver.UserId, sender.UserId);
            await Clients.All
                .SendAsync("UpdateUnreadCount", sender.Username, unreadCount);
        }
    }
}
