

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

        // images upload
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
                    AppUserId = CI.AppUserId
                }
            ).ToListAsync();
        }

        // raw for pdf, zip
        public async Task<todo_web_api.Models.UploadResult> CreateAsyncPdf(todo_web_api.Models.UploadResult UP)
        {
            await _context.UploadResults.AddAsync(UP);
            await _context.SaveChangesAsync();
            return UP;
        }

        public async Task<List<todo_web_api.Models.UploadResult>> GetPdfs(AppUser user)
        {
            return await _context.UploadResults.Where(u => u.AppUserId == user.Id).Select(
                UP => new todo_web_api.Models.UploadResult
                {
                    Id = UP.Id,
                    Url = UP.Url,
                    ResourceType = UP.ResourceType,
                    // AppUser = UP.AppUser,
                    AppUserId = UP.AppUserId
                }
            ).ToListAsync();
        }

    }

}