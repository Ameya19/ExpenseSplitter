using ExpenseSplitter.Core.DTOs.User;
using ExpenseSplitter.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
            {
                return Unauthorized();
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            {
                user.Id,
                user.DisplayName,
                user.Email,
                user.AvatarUrl,
                user.CreatedAt
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)
            {
                return Unauthorized();
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound("User not found.");
            }

            user.DisplayName = profileDto.DisplayName ?? user.DisplayName;
            user.AvatarUrl = profileDto.AvatarUrl ?? user.AvatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await this.userManager.UpdateAsync(user);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors
                .Select(e => e.Description));
            }

            return Ok(new
            {
                user.Id,
                user.DisplayName,
                user.Email,
                user.AvatarUrl
            });
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await this.userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors
                .Select(e => e.Description));
            }
            return Ok("Password changed successfully.");
        }
    }
}
