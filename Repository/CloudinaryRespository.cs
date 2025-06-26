

using Microsoft.EntityFrameworkCore;
using todo_web_api.Data;
using todo_web_api.Interface;
using todo_web_api.Models;

namespace todo_web_api.Respository
{
    public class CloudinaryRespository : ICloudinaryRespository
    {
        private readonly ApplicationDbContext _context;

        public CloudinaryRespository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CloudinaryImage> CreateAsync(CloudinaryImage CI)
        {
            await _context.cloudinaryImages.AddAsync(CI);
            await _context.SaveChangesAsync();
            return CI;
        }

        public async Task<List<CloudinaryImage>> GetCloudinary(AppUser user)
        {
            return await _context.cloudinaryImages.Where(u => u.AppUserId == user.Id).Select(
                CI => new CloudinaryImage
                {
                    Id = CI.Id,
                    ImageUrl = CI.ImageUrl,
                }
            ).ToListAsync();
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
    }

}