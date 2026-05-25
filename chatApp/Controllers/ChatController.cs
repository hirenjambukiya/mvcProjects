using System.Security.Claims;
using chatApp.Models;
using ChatApp.DTOs;
using ChatApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public ChatController(IUserService userService, IMessageService messageService)
        {
            _userService = userService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string receiverUsername)
        {
            var currentUserId = GetCurrentUserId();
            var reciver = await _userService.GetUserByNameAsync(receiverUsername);

            await _messageService.MarkMessageAsReadAsync(currentUserId,reciver.UserId);

            var messages = await _messageService.GetChatHistoryAsync(currentUserId, reciver.UserId);
            return Json(new
            {
                Messages = messages,
                Receiver = reciver
            });
        }
        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();
            var users = await _userService.GetAllExceptSelfAsync(currentUserId);

            var unreadCounts = await _messageService.GetUnreadCountsForAllUsersAsync(currentUserId);


            var userList = users.Select(user => new UserDto
            {
                UnreadCount = unreadCounts.FirstOrDefault(uc => uc.SenderId == user.UserId)?.Count ?? 0,
                Id = user.UserId,
                DisplayName = user.DisplayName,
                Username = user.Username,
                IsOnline = user.IsOnline,
                LastActive = user.LastActive.ToString("g")
            }).ToList();

            return View(userList);
        }

        public async Task<IActionResult> Chat(Guid withUserId)
        {
            var currentUserId = GetCurrentUserId();
            var chatPartner = await _userService.GetUserByIdAsync(withUserId);
            var messages = await _messageService.GetChatHistoryAsync(currentUserId, withUserId);

            if (chatPartner == null)
                return NotFound();

            var dto = new ChatUserWithMessagesDto
            {
                ChatPartner = new UserDto
                {
                    Id = chatPartner.UserId,
                    DisplayName = chatPartner.DisplayName,
                    Username = chatPartner.Username,
                    IsOnline = chatPartner.IsOnline,
                    LastActive = chatPartner.LastActive.ToString("g")
                },
                Messages = messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    SentAt = m.Timestamp.ToString("g"),
                    IsRead = m.IsRead,
                    IsSentByCurrentUser = m.SenderId == currentUserId 
                }).ToList()
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Guid receiverId, string messageText)
        {
            var senderId = GetCurrentUserId();

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = messageText,
                Timestamp = DateTime.Now,
                IsRead = false
            };

            await _messageService.SendMessageAsync(message);
            return RedirectToAction("Chat", new { withUserId = receiverId });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCounts()
        {
            var currentUser = await _userService.GetUserByNameAsync(User.Identity.Name);
            var result = await _messageService.GetUnreadCountsForAllUsersAsync(currentUser.UserId);
            return Json(result);
        }
    }
}
