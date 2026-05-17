using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Entities
{
    public class ExpenseSplit : BaseEntity
    {
        public Guid ExpenseId { get; set; }
        public Guid UserId { get; set; }
        public decimal ShareAmount { get; set; }
        public decimal? SharePercentage { get; set; }

        public Expense Expense { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
