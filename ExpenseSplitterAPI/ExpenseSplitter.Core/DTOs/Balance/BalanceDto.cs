using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.DTOs.Balance
{
    public class BalanceDto
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
        public decimal NetBalance  { get; set; }
        public string Status => NetBalance > 0 ? "IsOwed"
                         : NetBalance < 0 ? "Owes"
                         : "Settled";
    }
}
