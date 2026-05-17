using ExpenseSplitter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Interfaces.Repositories
{
    public interface ISettlementRepository
    {
        public Task<Settlement> RecordSettlement(Settlement settlement);
        public Task<IEnumerable<Settlement>> GetSettlementsByGroupId(Guid groupId);
        public Task<Settlement?> GetSettlementById(Guid settlementId);
        public Task<IEnumerable<Settlement>> GetSettlementsByUserId(Guid userId);
        public Task<Settlement?> CompleteSettlement(Guid settlementId);
        public Task<bool> CancelSettlement(Guid settlementId);
    }
}
