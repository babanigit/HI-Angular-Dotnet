

using todo_web_api.Models;

namespace todo_web_api.Interface
{
    public interface ICloudinaryRespository
    {
        Task<List<CloudinaryImage>> GetCloudinary(AppUser user);
        Task<CloudinaryImage> CreateAsync(CloudinaryImage CI);
    }
}