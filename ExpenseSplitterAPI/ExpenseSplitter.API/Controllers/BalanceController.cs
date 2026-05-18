using ExpenseSplitter.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.API.Controllers
{
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceRepository balanceRepository;

        public BalanceController(IBalanceRepository balanceRepository)
        {
            this.balanceRepository = balanceRepository;
        }

        [HttpGet("group/{groupId:Guid}")]
        public async Task<IActionResult> GetGroupBalances([FromRoute]Guid groupId)
        {
            var balances = await this.balanceRepository.GetGroupBalances(groupId);

            if (!balances.Any())
            {
                return NotFound("No expense found for this group.");
            }

            return Ok(balances);
        }

        [HttpGet("group/{groupId:Guid}/suggestions")]
        public async Task<IActionResult> GetSettlementSuggestions([FromRoute]Guid groupId)
        {
            var suggestions = await this.balanceRepository.GetSettlementSuggestions(groupId);

            if (!suggestions.Any())
            {
                return NotFound("Everyone is settled up!");
            }

            return Ok(suggestions);
        }
    }
}
