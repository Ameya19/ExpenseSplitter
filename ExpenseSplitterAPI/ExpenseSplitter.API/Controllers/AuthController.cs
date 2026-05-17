using ExpenseSplitter.Core.DTOs.Auth;
using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseSplitter.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly JwtTokenGenerator jwtTokenGenerator;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JwtTokenGenerator jwtTokenGenerator)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            var existingUser = await this.userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already in use.");
            }

            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            var token = jwtTokenGenerator.GenerateToken(user);

            var response = new AuthResponseDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = token,
                UserId = user.Id
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return BadRequest("Invalid email or password.");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded)
            {
                return BadRequest("Invalid email or password.");
            }

            var token = jwtTokenGenerator.GenerateToken(user);

            var response = new AuthResponseDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = token,
                UserId = user.Id
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }

            var response = new
            {
                DisplayName = user.DisplayName,
                Id = userId,
                Email = user.Email
            };

            return Ok(response);
        }
    }
}
