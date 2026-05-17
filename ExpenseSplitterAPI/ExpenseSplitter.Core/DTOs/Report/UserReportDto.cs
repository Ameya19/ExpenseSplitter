using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.DTOs.Report
{
    public class UserReportDto
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public decimal TotalPaidAcrossGroups { get; set; }
        public decimal TotalOwedAcrossGroups { get; set; }
        public decimal NetBalanceAcrossGroups { get; set; }
        public List<GroupSummaryDto> GroupSummaries { get; set; } = [];
        public List<CategoryBreakdownDto> CategoryBreakdown { get; set; } = [];
    }

    public class GroupSummaryDto
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public decimal TotalPaid { get; set; }
        public decimal NetBalance { get; set; }
    }
}
