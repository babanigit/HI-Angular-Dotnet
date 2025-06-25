

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using todo_web_api.Data;
using todo_web_api.Dtos;
using todo_web_api.Interface;
using todo_web_api.Models;

namespace todo_web_api.Respository
{
    public class TodoRespository : ITodoRespository
    {
        private readonly ApplicationDbContext _context;
        public TodoRespository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Todo>> GetUserTodo(AppUser user)
        {
            // throw new NotImplementedException();

            return await _context.Todos.Where(u => u.AppUserId == user.Id).Select(
                todo => new Todo
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Text = todo.Text,
                    Status = todo.Status,
                    CreatedAt = todo.CreatedAt,
                    UpdatedAt = todo.UpdatedAt,
                    DueDate = todo.DueDate,
                    IsImportant = todo.IsImportant,
                    CompletedAt = todo.CompletedAt,
                    IsDeleted = todo.IsDeleted
                    
                    // AppUserId = todo.AppUserId,
                    // AppUser = todo.AppUser
                }
            ).ToListAsync();
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            // throw new NotImplementedException();
            await _context.Todos.AddAsync(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> DeleteTodoAsync(AppUser user, int todoId)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.AppUserId == user.Id);

            if (todo == null)
                return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Todo?> UpdateTodoAsync(AppUser user, int todoId, UpdateTodoDto dto)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.AppUserId == user.Id);

            if (todo == null) return null;


            Console.WriteLine("========== BEFORE UPDATE ==========");
            Console.WriteLine($"DB STATUS: {todo.Status}");
            Console.WriteLine($"DTO STATUS: {dto.Status}");

                //     Status = todo.Status,
                //     CreatedAt = todo.CreatedAt,
                //     UpdatedAt = todo.UpdatedAt,
                //     DueDate = todo.DueDate,
                //     IsImportant = todo.IsImportant,
                //     CompletedAt = todo.CompletedAt,
                //     IsDeleted = todo.IsDeleted

            todo.Title = dto.Title;
            todo.Text = dto.Text;
            todo.Status = dto.Status;
            todo.DueDate = dto.DueDate;
            todo.IsImportant = dto.IsImportant;
            

            _context.Entry(todo).Property(t => t.Status).IsModified = true;
            await _context.SaveChangesAsync();

            Console.WriteLine("========== AFTER UPDATE ==========");
            Console.WriteLine($"DB STATUS AFTER SAVE: {todo.Status}");

            return todo;
        }

    }

}