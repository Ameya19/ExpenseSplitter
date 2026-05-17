using ExpenseSplitter.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public Guid? ReferenceId { get; set; }   // GroupId or ExpenseId
        public string? ReferenceType { get; set; } // "Group" | "Expense"

        // Navigation
        public User User { get; set; } = null!;
    }
}
