using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.DTOs.Report
{
    public class GroupReportDto
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalTransactions { get; set; }
        public List<CategoryBreakdownDto> CategoryBreakDown { get; set; } = [];
        public List<MemberSpendingDto> MemberSpending { get; set; } = [];
        public List<MonthlyBreakdownDto> MonthlyBreakdownDto { get; set; } = [];
    }

    public class CategoryBreakdownDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MemberSpendingDto
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public decimal TotalPaid { get; set; }
        public decimal TotalOwed { get; set; }
        public decimal NetBalance { get; set; }
    }

    public class MonthlyBreakdownDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalTransactions { get; set; }
    }
}
