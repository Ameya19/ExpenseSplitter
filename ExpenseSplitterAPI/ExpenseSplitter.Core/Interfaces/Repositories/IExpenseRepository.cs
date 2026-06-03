using ExpenseSplitter.Core.DTOs.Expense;
using ExpenseSplitter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Interfaces.Repositories
{
    public interface IExpenseRepository
    {
        public Task<Expense> AddExpenses(Expense expense, CreateExpenseDto dto);
        public Task<IEnumerable<Expense>> GetAllExpenses();
        public Task<Expense?> GetExpenseById(Guid id);
        public Task<IEnumerable<Expense>> GetExpensesByGroup(Guid id);
        public Task<IEnumerable<Expense>> GetExpensesByUserId(Guid userId);
        public Task<bool> DeleteExpense(Guid id);
        public Task<Expense?> UpdateExpense(Guid id, Expense expense);
    }
}
