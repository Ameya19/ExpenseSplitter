using ExpenseSplitter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        public Task<Notification> CreateNotification(Notification notification);
        public Task<int> GetUnreadCount(Guid userId);
        public Task<IEnumerable<Notification>> GetMyNotifications(Guid userId);
        public Task<bool> MarkAsRead(Guid notificationId);
        public Task<bool> MarkAllAsRead(Guid userId);
        public Task<bool> DeleteNotification(Guid notificationId);
    }
}
