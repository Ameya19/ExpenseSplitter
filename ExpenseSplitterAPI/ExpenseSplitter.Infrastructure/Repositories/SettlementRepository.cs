using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Interfaces.Repositories;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class SettlementRepository : ISettlementRepository
    {
        private readonly AppDbContext appDbContext;

        public SettlementRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Settlement?> GetSettlementById(Guid settlementId)
        {
            return await this.appDbContext.Settlements
                .Include(s => s.FromUser)
                .Include(s => s.ToUser)
                .FirstOrDefaultAsync(s => s.Id == settlementId);
        }

        public async Task<IEnumerable<Settlement>> GetSettlementsByGroupId(Guid groupId)
        {
            return await this.appDbContext.Settlements
                .Include(s => s.FromUser)
                .Include(s => s.ToUser)
                .Where(s => s.GroupId == groupId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Settlement> RecordSettlement(Settlement settlement)
        {
            this.appDbContext.Settlements.Add(settlement);
            await this.appDbContext.SaveChangesAsync();
            return settlement;
        }

        public async Task<IEnumerable<Settlement>> GetSettlementsByUserId(Guid userId)
        {
            return await this.appDbContext.Settlements
                .Include(s => s.FromUser)
                .Include(s => s.ToUser)
                .Where(s => s.FromUserId == userId || s.ToUserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Settlement?> CompleteSettlement(Guid settlementId)
        {
            var settlement = await this.appDbContext.Settlements.FirstOrDefaultAsync(s => s.Id == settlementId);
            if (settlement != null)
            {
                settlement.Status = Core.Enums.SettlementStatus.Completed;
                settlement.SettledAt = DateTime.UtcNow;
                settlement.UpdatedAt = DateTime.UtcNow;
                await this.appDbContext.SaveChangesAsync();

                return settlement;
            }
            return null;
        }

        public async Task<bool> CancelSettlement(Guid settlementId)
        {
            var settlement = await this.appDbContext.Settlements.FirstOrDefaultAsync(s => s.Id == settlementId);

            if (settlement == null)
            {
                return false;
            }
            else
            {
                settlement.Status = Core.Enums.SettlementStatus.Cancelled;
                settlement.UpdatedAt = DateTime.UtcNow;
                this.appDbContext.SaveChanges();
                return true;
            }
        }
    }
}
