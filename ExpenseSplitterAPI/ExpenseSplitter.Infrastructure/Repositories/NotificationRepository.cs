using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Interfaces.Repositories;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext appDbContext;

        public NotificationRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Notification> CreateNotification(Notification notification)
        {
            this.appDbContext.Notifications.Add(notification);
            await this.appDbContext.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> DeleteNotification(Guid notificationId)
        {
            var notification = await this.appDbContext.Notifications.Where(n => n.Id == notificationId).FirstOrDefaultAsync();

            if (notification == null)
            {
                return false;
            }
            else
            {
                this.appDbContext.Notifications.Remove(notification);
                await this.appDbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<IEnumerable<Notification>> GetMyNotifications(Guid userId)
        {
            return await this.appDbContext.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<int> GetUnreadCount(Guid userId)
        {
            return await this.appDbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).CountAsync();
        }

        public async Task<bool> MarkAllAsRead(Guid userId)
        {
            var notifications = await this.appDbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();

            if (notifications == null)
            {
                return false;
            }
            else
            {
                foreach(var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.UpdatedAt = DateTime.UtcNow;
                }
                await this.appDbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> MarkAsRead(Guid notificationId)
        {
            var notification = await this.appDbContext.Notifications.Where(n => n.Id == notificationId && !n.IsRead).FirstOrDefaultAsync();

            if(notification == null)
            {
                return false;
            }
            else
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await this.appDbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
