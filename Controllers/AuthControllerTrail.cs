using Microsoft.AspNetCore.Mvc;
using todo_web_api.Models;
using todo_web_api.Services;

namespace todo_web_api.Controllers
{
    [ApiController]
    [Route("api/auth")]

    public class AuthControllerTrail : ControllerBase
    {
        public readonly UserServiceTrail _userServiceTrail;
        public AuthControllerTrail(UserServiceTrail userServiceTrail)
        {
            _userServiceTrail = userServiceTrail;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserTrail user)
        {
            if (_userServiceTrail.FindByEmail(user.Email) is not null)
                return BadRequest("User already exists hehe");

            _userServiceTrail.Add(user);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserTrail login)
        {
            var user = _userServiceTrail.FindByEmail(login.Email);
            if (user is null || user.Password != login.Password)
            {
                Console.WriteLine($" bab :- login unsuccess  {user} ");

                return Unauthorized("Invalid email or password");

            }

            Console.WriteLine($" bab :- login success ");

            return Ok("Login successful");
        }

    }
}