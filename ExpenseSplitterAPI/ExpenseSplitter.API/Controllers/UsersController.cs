using ExpenseSplitter.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public UsersController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> getUserByEmail([FromQuery] string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("No user found with this Email.");
            }
            else
            {
                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.DisplayName
                });
            }
        }
    }
}
