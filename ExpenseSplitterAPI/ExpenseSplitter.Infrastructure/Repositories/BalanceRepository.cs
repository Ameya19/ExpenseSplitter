using ExpenseSplitter.Core.DTOs.Balance;
using ExpenseSplitter.Core.Enums;
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
    public class BalanceRepository : IBalanceRepository
    {
        private readonly AppDbContext appDbContext;

        public BalanceRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<BalanceDto>> GetGroupBalances(Guid groupId)
        {
            var expenses = await this.appDbContext.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Splits)
                .ThenInclude(e => e.User)
                .Where(e => e.GroupId == groupId)
                .ToListAsync();

            var settlements = await this.appDbContext.Settlements
                .Where(e => e.GroupId == groupId && e.Status == SettlementStatus.Completed)
                .ToListAsync();

            var balances = new Dictionary<Guid, (decimal Amount, string Name)>();

            foreach (var expense in expenses)
            {
                if(!balances.ContainsKey(expense.PaidByUserId))
                {
                    balances[expense.PaidByUserId] = (0, expense.PaidBy.DisplayName);
                }
                
                balances[expense.PaidByUserId] = (balances[expense.PaidByUserId].Amount + expense.Amount, expense.PaidBy.DisplayName);

                foreach (var split in expense.Splits)
                {
                    if (!balances.ContainsKey(split.UserId))
                    {
                        balances[split.UserId] = (0, split.User.DisplayName);
                    }

                    balances[split.UserId] = (balances[split.UserId].Amount - split.ShareAmount, split.User.DisplayName);

                }
            }

            foreach (var settlement in settlements)
            {
                if (balances.ContainsKey(settlement.FromUserId))
                {
                    balances[settlement.FromUserId] = (balances[settlement.FromUserId].Amount + settlement.Amount, balances[settlement.FromUserId].Name);
                }

                if(balances.ContainsKey(settlement.ToUserId))
                {
                    balances[settlement.ToUserId] = (balances[settlement.ToUserId].Amount + settlement.Amount, balances[settlement.ToUserId].Name);
                }
            }

            return balances.Select(b => new BalanceDto
            {
                UserId = b.Key,
                DisplayName = b.Value.Name,
                NetBalance = Math.Round(b.Value.Amount, 2)
            }).ToList();
        }

        public async Task<List<SettlementSuggestionDto>> GetSettlementSuggestions(Guid groupId)
        {
            var balances = await GetGroupBalances(groupId);

            var creditors = balances.Where(b => b.NetBalance > 0)
                .Select(b => new
                {
                    b.UserId,
                    b.DisplayName,
                    Amount = b.NetBalance
                })
                .OrderByDescending(b => b.Amount)
                .ToList();

            var debtors = balances.Where(b => b.NetBalance < 0)
                .Select(b => new
                {
                    b.UserId,
                    b.DisplayName,
                    Amount = Math.Abs(b.NetBalance)
                })
                .OrderByDescending(b => b.Amount)
                .ToList();

            var suggestions = new List<SettlementSuggestionDto>();

            var creditorAmounts = creditors.Select(s => s.Amount).ToList();
            var debtorAmounts = debtors.Select(d => d.Amount).ToList();

            int i = 0, j = 0;

            while(i < creditors.Count && j< debtors.Count)
            {
                var amount = Math.Min(creditorAmounts[i], debtorAmounts[j]);

                suggestions.Add(new SettlementSuggestionDto
                {
                    FromUserId = debtors[j].UserId,
                    FromUserName = debtors[j].DisplayName,
                    ToUserId = creditors[i].UserId,
                    ToUserName = creditors[i].DisplayName,
                    Amount = Math.Round(amount, 2)
                });

                creditorAmounts[i] -= amount;
                debtorAmounts[j] -= amount;

                if (creditorAmounts[i] == 0) i++;
                if (debtorAmounts[j] == 0) j++;
            }

            return suggestions;
        }
    }
}
