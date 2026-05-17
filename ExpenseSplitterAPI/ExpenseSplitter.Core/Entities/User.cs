using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public ICollection<GroupMember> GroupMembers { get; set; } = [];
        public ICollection<Expense> PaidExpenses { get; set; } = [];
        public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = [];

        public ICollection<Settlement> SentSettlements { get; set; } = [];
        public ICollection<Settlement> ReceivedSettlements { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
    }
}
