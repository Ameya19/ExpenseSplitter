using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Enums;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Infrastructure.Helpers
{
    public class NotificationHelper
    {
        private readonly AppDbContext appDbContext;

        public NotificationHelper(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task NotifyGroupMembers(Guid groupId, Guid excludeUserId, NotificationType type, string message, string title)
        {
            var memberIds = await appDbContext.GroupMembers.Where(gm => gm.GroupId == groupId && gm.UserId != excludeUserId).Select(m => m.UserId).ToListAsync();

            var notifications = memberIds.Select(memberId => new Notification
            {
                UserId = memberId,
                Title = title,
                Message = message,
                Type = type,
                ReferenceId = groupId,
                ReferenceType = "Group",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await appDbContext.Notifications.AddRangeAsync(notifications);
            await appDbContext.SaveChangesAsync();
        }

        public async Task NotifyUser(Guid userId, NotificationType type, string message, string title, Guid? referenceId = null, string? referenceType = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await appDbContext.Notifications.AddAsync(notification);
            await appDbContext.SaveChangesAsync();
        }
    }
}
