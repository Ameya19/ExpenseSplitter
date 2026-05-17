using ExpenseSplitter.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Entities
{
    public class Expense : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Guid PaidByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public SplitType SplitType { get; set; }
        public DateTime Date { get; set; }

        public Group Group { get; set; } = null!;
        public User PaidBy { get; set; } = null!;
        public ICollection<ExpenseSplit> Splits { get; set; } = [];
    }
}
