using ExpenseSplitter.Core.DTOs.Expense;
using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Interfaces.Repositories;
using ExpenseSplitter.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.API.Controllers
{
    //https://localhost:xxxx/api/expenses
    [Route("api/expenses")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseRepository expenseRepository;

        public ExpensesController(IExpenseRepository expenseRepository)
        {
            this.expenseRepository = expenseRepository;
        }

        //https://localhost:7194/api/expenses
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddExpenses([FromBody]CreateExpenseDto expenseDto)
        {
            var expenses = new Expense
            {
                Amount = expenseDto.Amount,
                Title = expenseDto.Title,
                Category = expenseDto.Category,
                SplitType = expenseDto.SplitType,
                Date = expenseDto.Date,
                GroupId = expenseDto.GroupId,
                PaidByUserId = expenseDto.PaidByUserId
            };

            var response = await this.expenseRepository.AddExpenses(expenses);

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            var allExpenses = await this.expenseRepository.GetAllExpenses();
            var response = new List<Expense>();

            if(allExpenses == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var expense in allExpenses)
                {
                    response.Add(new Expense
                    {
                        Amount = expense.Amount,
                        Title = expense.Title,
                        GroupId = expense.GroupId,
                        Group = expense.Group,
                        PaidByUserId = expense.PaidByUserId,
                        Category = expense.Category,
                        SplitType = expense.SplitType,
                        Date = expense.Date,
                        PaidBy = expense.PaidBy,
                        Splits = expense.Splits
                    });
                }

                return Ok(response);
            }
        }

        //https://localhost:7194/api/expenses/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetExpenseById([FromRoute] Guid id)
        {
            var expenseDetails = await this.expenseRepository.GetExpenseById(id);

            if (expenseDetails == null)
            {
                return NotFound();
            }
            else
            {
                var response = new Expense
                {
                    Amount = expenseDetails.Amount,
                    Title = expenseDetails.Title,
                    GroupId = expenseDetails.GroupId,
                    Group = expenseDetails.Group,
                    PaidByUserId = expenseDetails.PaidByUserId,
                    Category = expenseDetails.Category,
                    SplitType = expenseDetails.SplitType,
                    Date = expenseDetails.Date,
                    PaidBy = expenseDetails.PaidBy,
                    Splits = expenseDetails.Splits,
                };

                return Ok(response);
            }
        }

        //https://localhost:7194/api/expenses/group/{groupId}
        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetExpensesByGroup(Guid groupId)
        {
            var groupExpenses = await this.expenseRepository.GetExpensesByGroup(groupId);

            var response = new List<Expense>();

            if (groupExpenses == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var expense in groupExpenses)
                {
                    response.Add(new Expense
                    {
                        Amount = expense.Amount,
                        Title = expense.Title,
                        GroupId = expense.GroupId,
                        Group = expense.Group,
                        PaidByUserId = expense.PaidByUserId,
                        Category = expense.Category,
                        SplitType = expense.SplitType,
                        Date = expense.Date,
                        PaidBy = expense.PaidBy,
                        Splits = expense.Splits
                    });
                }

                return Ok(response);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetExpensesByUserId(Guid userId)
        {
            var userExpenses = await this.expenseRepository.GetExpensesByUserId(userId);
            var response = new List<Expense>();

            if (userExpenses == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var expense in userExpenses)
                {
                    response.Add(new Expense
                    {
                        Amount = expense.Amount,
                        Title = expense.Title,
                        GroupId = expense.GroupId,
                        Group = expense.Group,
                        PaidByUserId = expense.PaidByUserId,
                        Category = expense.Category,
                        SplitType = expense.SplitType,
                        Date = expense.Date,
                        PaidBy = expense.PaidBy,
                        Splits = expense.Splits
                    });
                }

                return Ok(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            var deletedExpense = await this.expenseRepository.DeleteExpense(id);

            if (deletedExpense == true)
            {
                return Ok("Expense Deleted Successfully!");
            }
            else
            {
                return NotFound($"Expense {id} not found");
            }
        }

        //https://localhost:7194/expenses/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateExpense([FromRoute]Guid id, [FromBody]UpdateExpenseDto updateExpenseDto)
        {
            var updatedExpense = new Expense
            {
                Amount = updateExpenseDto.Amount,
                Title = updateExpenseDto.Title,
                Category = updateExpenseDto.Category,
                SplitType = updateExpenseDto.SplitType,
                Date = updateExpenseDto.Date,
            };

            var expense = await this.expenseRepository.UpdateExpense(id, updatedExpense);

            if(null == expense)
            {
                return NotFound();
            }
            else
            {
                var response = new Expense
                {
                    Amount = expense.Amount,
                    Title = expense.Title,
                    Category = expense.Category,
                    SplitType = expense.SplitType,
                    Date = expense.Date,
                    Id = id,
                    UpdatedAt = expense.UpdatedAt,
                };

                return Ok(response);
            }
        }
    }
}
