using ExpenseSplitter.Core.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Interfaces.Repositories
{
    public interface IReportRepository
    {
        public Task<GroupReportDto> GetGroupReport(Guid groupId);
        public Task<GroupReportDto> GetGroupReportByDateRange(Guid groupId, DateTime startDate, DateTime endDate);
        public Task<UserReportDto> GetUserReport(Guid userId);
    }
}
