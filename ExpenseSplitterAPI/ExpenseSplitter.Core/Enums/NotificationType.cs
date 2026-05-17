using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Enums
{
    public enum NotificationType
    {
        ExpenseAdded = 1,
        ExpenseUpdated = 2,
        ExpenseDeleted = 3,
        SettlementRequested = 4,
        SettlementCompleted = 5,
        GroupInvite = 6,
        MemberJoined = 7
    }
}
