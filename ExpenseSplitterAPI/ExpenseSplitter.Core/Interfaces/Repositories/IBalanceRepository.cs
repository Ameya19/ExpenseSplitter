using ExpenseSplitter.Core.DTOs.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Interfaces.Repositories
{
    public interface IBalanceRepository
    {
        public Task<List<BalanceDto>> GetGroupBalances(Guid groupId);
        public Task<List<SettlementSuggestionDto>> GetSettlementSuggestions(Guid groupId);
    }
}
