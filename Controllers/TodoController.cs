
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using todo_web_api.Dtos;
using todo_web_api.Extensions;
using todo_web_api.Interface;
using todo_web_api.Models;
using todo_web_api.Respository;

namespace todo_web_api.Controllers
{
    [Route("api/todo")]
    [ApiController]
    public class TodoController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITodoRespository _todoRepo;

        public TodoController(UserManager<AppUser> userManager, ITodoRespository todoRepo)
        {
            _userManager = userManager;
            _todoRepo = todoRepo;
        }

        // get the user todo
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserTodo()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var userTodo = await _todoRepo.GetUserTodo(appUser);

            return Ok(userTodo);
        }

        // here we have to create a todo
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTodo([FromBody] CreateTodoDto dto)
        {

            if (string.IsNullOrWhiteSpace(dto.Text) || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title and Text are required");

            // Get username from JWT
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return Unauthorized("User not found");

            // Check for duplicate todo by text
            var userTodos = await _todoRepo.GetUserTodo(appUser);

            if (userTodos.Any(e => e.Text.ToLower() == dto.Text.ToLower()))
                return BadRequest("A todo with the same text already exists");


            //  Create and save
            var todoModel = new Todo
            {
                Title = dto.Title,
                Text = dto.Text,
                // UpdatedAt =
                DueDate = dto.DueDate,
                IsImportant = dto.IsImportant,
                IsDeleted = dto.IsDeleted,
                CompletedAt = dto.CompletedAt,
                AppUserId = appUser.Id
            };

            await _todoRepo.CreateAsync(todoModel);

            if (todoModel == null)
                return StatusCode(500, "Could not create");
            else
                return CreatedAtAction(nameof(AddTodo), new { id = todoModel.Id }, todoModel);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return Unauthorized("User not found");

            var deleted = await _todoRepo.DeleteTodoAsync(appUser, id);

            if (!deleted)
                return NotFound("Todo not found or doesn't belong to the user");

            return Ok("Todo Deleted"); // 204
        }


        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest("Title and Text are required");

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return Unauthorized("User not found");

            var updatedTodo = await _todoRepo.UpdateTodoAsync(appUser, id, dto);




            Console.WriteLine($"the updated Todo from the backend is :-  {updatedTodo!.Status}");

            if (updatedTodo == null)
                return NotFound("Todo not found or doesn't belong to the user");

            return Ok(updatedTodo);
        }


    }
}
