using ExpenseSplitter.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.DTOs.Expense
{
    public class UpdateExpenseDto
    {
        public string Title{ get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public SplitType SplitType { get; set; }
        public DateTime Date {  get; set; }

    }
}
