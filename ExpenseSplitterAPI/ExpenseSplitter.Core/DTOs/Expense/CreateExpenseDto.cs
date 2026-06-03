using ExpenseSplitter.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.DTOs.Expense
{
    public class CreateExpenseDto
    {
        public Guid GroupId { get; set; }
        public Guid PaidByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public SplitType SplitType { get; set; }
        public DateTime Date { get; set; }
        public List<ExpenseSplitInputDto> Splits { get; set; } = [];
    }

    public class  ExpenseSplitInputDto 
    {
        public Guid UserId { get; set; }
        public decimal? ShareAmount { get; set; }
        public decimal? SharePercentage { get; set; }
    }
}
