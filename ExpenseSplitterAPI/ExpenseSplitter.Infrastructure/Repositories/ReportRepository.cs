using ExpenseSplitter.Core.DTOs.Report;
using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Interfaces.Repositories;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = ExpenseSplitter.Core.Entities.Group;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext appDbContext;

        public ReportRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public GroupReportDto BuildGroupReport(Group group, List<Expense> expenses)
        {
            var totalAmount = expenses.Sum(expense => expense.Amount);

            var categoryBreakDown = expenses
                .GroupBy(e => e.Category)
                .Select(g => new CategoryBreakdownDto
                {
                    Category = g.Key.ToString(),
                    TotalAmount = Math.Round(g.Sum(e => e.Amount), 2),
                    Count = g.Count(),
                    Percentage = totalAmount > 0 ? Math.Round(g.Sum(e => e.Amount) / totalAmount * 100, 2) : 0
                })
                .OrderByDescending(c => c.TotalAmount)
                .ToList();

            var memberSpending = expenses
                .GroupBy(e => new
                {
                    e.PaidBy.DisplayName,
                    e.PaidByUserId
                })
                .Select(g => new MemberSpendingDto
                {
                    UserId = g.Key.PaidByUserId,
                    DisplayName = g.Key.DisplayName,
                    TotalPaid = Math.Round(g.Sum(e => e.Amount), 2),
                    TotalOwed = Math.Round(
                        expenses.SelectMany(e => e.Splits)
                        .Where(s => s.UserId == g.Key.PaidByUserId)
                        .Sum(s => s.ShareAmount), 2),
                    NetBalance = Math.Round(g.Sum(e => e.Amount) - expenses.SelectMany(e => e.Splits).Where(e => e.UserId == g.Key.PaidByUserId).Sum(s => s.ShareAmount), 2)
                })
                .OrderByDescending(m => m.TotalPaid)
                .ToList();

            var monthlyBreakdown = expenses.
                GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new MonthlyBreakdownDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    TotalAmount = Math.Round(g.Sum(e => e.Amount), 2),
                    TotalTransactions = g.Count()
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .ToList();

            var response = new GroupReportDto
            {
                GroupId = group.Id,
                GroupName = group.Name,
                TotalAmount = Math.Round(totalAmount, 2),
                TotalTransactions = expenses.Count,
                CategoryBreakdown = categoryBreakDown,
                MemberSpending = memberSpending,
                MonthlyBreakdown = monthlyBreakdown
            };

            return response;
        }

        public async Task<GroupReportDto> GetGroupReport(Guid groupId)
        {
            var group = await this.appDbContext.Groups.FirstOrDefaultAsync(x => x.Id == groupId);

            var expenses = await this.appDbContext.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Splits)
                .Where(e => e.GroupId == groupId)
                .ToListAsync();

            return BuildGroupReport(group!, expenses);
        }

        public async Task<GroupReportDto> GetGroupReportByDateRange(Guid groupId, DateTime startDate, DateTime endDate)
        {
            var group = await this.appDbContext.Groups.FirstOrDefaultAsync(x => x.Id == groupId);

            var expenses = await this.appDbContext.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Splits)
                .Where(e => e.GroupId == groupId && e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();

            return BuildGroupReport(group!, expenses);
        }

        public async Task<UserReportDto> GetUserReport(Guid userId)
        {
            var user = await this.appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            // Get all groups user belongs to
            var groupIds = await this.appDbContext.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId)
                .ToListAsync();

            // Get all expenses user is involved in
            var expenses = await this.appDbContext.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Splits)
                .Include(e => e.Group)
                .Where(e => groupIds.Contains(e.GroupId) && (e.PaidByUserId == userId || e.Splits.Any(s => s.UserId == userId)))
                .ToListAsync();

            // Total paid across all groups
            var totalPaid = expenses.Where(e => e.PaidByUserId == userId).Sum(e => e.Amount);

            // Total owed across all groups
            var totalOwed = expenses.SelectMany(e => e.Splits)
                .Where(s => s.UserId == userId)
                .Sum(s => s.ShareAmount);

            // Per group summaries
            var groupSummaries = expenses.GroupBy(e => new { e.GroupId, e.Group.Name })
                .Select(g => new GroupSummaryDto
                {
                    GroupId = g.Key.GroupId,
                    GroupName = g.Key.Name,
                    TotalPaid = g.Where(e => e.PaidByUserId == userId).Sum(e => e.Amount),
                    NetBalance = g.Where(e => e.PaidByUserId == userId).Sum(e => e.Amount) - g.SelectMany(e => e.Splits).Where(s => s.UserId == userId).Sum(s => s.ShareAmount)
                }).ToList();

            // Category Breakdown for user
            var categoryBreakDown = expenses.Where(e => e.PaidByUserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new CategoryBreakdownDto
                {
                    Category = g.Key.ToString(),
                    TotalAmount = g.Sum(e => e.Amount),
                    Count = g.Count(),
                    Percentage = totalPaid > 0 ? Math.Round(g.Sum(e => e.Amount) / totalPaid * 100, 2) : 0
                }).ToList();

            var userReportDto = new UserReportDto
            {
                UserId = userId,
                DisplayName = user!.DisplayName,
                TotalPaidAcrossGroups = totalPaid,
                TotalOwedAcrossGroups = totalOwed,
                NetBalanceAcrossGroups = totalPaid - totalOwed,
                GroupSummaries = groupSummaries,
                CategoryBreakdown = categoryBreakDown
            };

            return userReportDto;
        }
    }
}
