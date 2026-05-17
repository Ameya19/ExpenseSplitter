using ExpenseSplitter.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Entities
{
    public class Settlement : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public SettlementStatus Status { get; set; } = SettlementStatus.Pending;
        public DateTime? SettledAt { get; set; }

        public Group Group { get; set; } = null!;
        public User FromUser { get; set; } = null!;
        public User ToUser { get; set; } = null!;
    }
}
