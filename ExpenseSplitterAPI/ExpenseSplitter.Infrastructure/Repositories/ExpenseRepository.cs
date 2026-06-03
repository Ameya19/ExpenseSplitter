using ExpenseSplitter.Core.DTOs.Expense;
using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Interfaces.Repositories;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext appDbContext;

        public ExpenseRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Expense> AddExpenses(Expense expense, CreateExpenseDto dto)
        {
            expense.Splits = BuildSplits(dto);

            await appDbContext.AddAsync(expense);
            await appDbContext.SaveChangesAsync();
            return expense;
        }

        public List<ExpenseSplit> BuildSplits(CreateExpenseDto dto)
        {
            return dto.SplitType switch
            {
                // Equal split: Each user pays an equal share of the total amount.
                Core.Enums.SplitType.Equal => dto.Splits.Select(s => new ExpenseSplit
                {
                    ExpenseId = Guid.Empty,
                    UserId = s.UserId,
                    ShareAmount = Math.Round(dto.Amount / dto.Splits.Count, 2),
                }).ToList(),

                // Percentage split: Each user pays a share of the total amount based on a specified percentage.
                Core.Enums.SplitType.Percentage => dto.Splits.Select(s => new ExpenseSplit
                {
                    UserId = s.UserId,
                    ShareAmount = Math.Round(dto.Amount * (s.SharePercentage!.Value) / 100, 2),
                    SharePercentage = s.SharePercentage
                }).ToList(),
                Core.Enums.SplitType.Exact => dto.Splits.Select(s => new ExpenseSplit
                {
                    UserId = s.UserId,
                    ShareAmount = s.ShareAmount!.Value
                }).ToList(),

                _ => []
            };
        }

        public async Task<bool> DeleteExpense(Guid id)
        {
            var expenseToBeDeleted = await appDbContext.Expenses.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (expenseToBeDeleted == null)
            {
                return false;
            }
            else
            {
                expenseToBeDeleted.IsDeleted = true;
                expenseToBeDeleted.UpdatedAt = DateTime.UtcNow;

                await appDbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<IEnumerable<Expense>> GetAllExpenses()
        {
            return await appDbContext.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Group)
                .Include(e => e.Splits)
                .ThenInclude(e => e.User)
                .ToListAsync();
        }

        public async Task<Expense?> GetExpenseById(Guid id)
        {
            return await appDbContext.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Group)
                .Include(e => e.Splits)
                .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Expense>> GetExpensesByGroup(Guid id)
        {
            return await appDbContext.Expenses.Where(x => x.GroupId == id).ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetExpensesByUserId(Guid userId)
        {
            return await appDbContext.Expenses.Where(x => x.PaidByUserId == userId).ToListAsync();
        }

        public async Task<Expense?> UpdateExpense(Guid id,Expense updatedExpense)
        {
            var expense = await appDbContext.Expenses.FirstOrDefaultAsync(x => x.Id == id);

            if (expense == null)
            {
                return null;
            }
            else
            {
                expense.Title = updatedExpense.Title;
                expense.Amount = updatedExpense.Amount;
                expense.Category = updatedExpense.Category;
                expense.SplitType = updatedExpense.SplitType;
                expense.Date = updatedExpense.Date;

                await appDbContext.SaveChangesAsync();
                return expense;
            }
        }
    }
}
