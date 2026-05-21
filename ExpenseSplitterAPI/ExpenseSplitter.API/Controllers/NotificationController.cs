using ExpenseSplitter.Core.DTOs.Notifications;
using ExpenseSplitter.Core.Interfaces.Repositories;
using ExpenseSplitter.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseSplitter.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto createNotificationDto)
        {
            var notification = new Core.Entities.Notification
            {
                UserId = createNotificationDto.UserId,
                Title = createNotificationDto.Title,
                Message = createNotificationDto.Message,
                Type = createNotificationDto.Type,
                ReferenceId = createNotificationDto.ReferenceId,
                ReferenceType = createNotificationDto.ReferenceType,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var response = await this.notificationRepository.CreateNotification(notification);

            return Ok(response);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var count = await this.notificationRepository.GetUnreadCount(userId);

            return Ok(new { UnreadCount = count });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications = await this.notificationRepository.GetMyNotifications(userId);

            if (notifications == null)
            {
                return NotFound("No Notifications found!");
            }
            else
            {
                var response = new List<NotificationResponseDto>();

                foreach (var notification in notifications)
                {
                    var notificationDetails = new NotificationResponseDto
                    {
                        Id = notification.Id,
                        Title = notification.Title,
                        Message = notification.Message,
                        Type = notification.Type.ToString(),
                        IsRead = notification.IsRead,
                        ReferenceId = notification.ReferenceId,
                        ReferenceType = notification.ReferenceType,
                        CreatedAt = notification.CreatedAt
                    };

                    response.Add(notificationDetails);
                }

                return Ok(response);
            }
        }

        [HttpPut("{id:Guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await this.notificationRepository.MarkAsRead(id);
            if (result)
            {
                return Ok("Notification marked as read.");
            }
            else
            {
                return NotFound("Notification not found!");
            }
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await this.notificationRepository.MarkAllAsRead(userId);

            if (result)
            {
                return Ok("All notifications marked as read.");
            }
            else
            {
                return NotFound("No notifications found!");
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var result = await this.notificationRepository.DeleteNotification(id);
            if (result)
            {
                return Ok("Notification deleted successfully.");
            }
            else
            {
                return NotFound("Notification not found!");
            }
        }
    }
}
