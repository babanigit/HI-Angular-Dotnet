using Finshark.Dtos.Account;
using Finshark.DTOs.accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_web_api.Extensions;
using todo_web_api.Interface;
using todo_web_api.Models;
using todo_web_api.Services;

namespace todo_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _tokenService = tokenService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    return Ok(
                        new NewUserDto
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                        }
                    );
                }
                else
                    return BadRequest(createdUser.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error:- " + ex);

            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalizedUsername = loginDto.Username.ToUpper();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUsername);

            if (user == null) return Unauthorized("Invalid username!");

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            var token = _tokenService.CreateToken(user);

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = token
                }
            );
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> Logged_in_User()
        {
            try
            {
                var username = User.GetUsername();
                var appUser = await _userManager.FindByNameAsync(username);

                if (string.IsNullOrEmpty(appUser!.Id))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                // Find the user in database
                var user = await _userManager.FindByIdAsync(appUser.Id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Return user information (excluding sensitive data)
                var userResponse = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    // FirstName = user.FirstName, // If you have these properties
                    // LastName = user.LastName,   // If you have these properties
                    // EmailConfirmed = user.EmailConfirmed,
                    // PhoneNumber = user.PhoneNumber,
                    // Roles = roles,
                    // CreatedAt = user.CreatedAt, // If you have this property
                    // LastLoginAt = user.LastLoginAt // If you have this property
                };

                return Ok(new
                {
                    success = true,
                    message = "User retrieved successfully",
                    data = userResponse
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving user information",
                    error = ex.Message // Remove this in production
                });
            }

        }

    }
}
