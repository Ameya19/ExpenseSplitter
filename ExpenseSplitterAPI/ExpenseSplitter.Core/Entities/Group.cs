using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Entities
{
    public class Group : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid CreatedByUserId { get; set; }
        public string? Description { get; set; }

        public User CreatedBy { get; set; } = null!;
        public ICollection<GroupMember> Members { get; set; } = [];
        public ICollection<Expense> Expenses { get; set; } = [];
    }
}
