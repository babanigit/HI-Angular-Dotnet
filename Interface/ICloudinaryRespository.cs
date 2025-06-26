

using todo_web_api.Models;

namespace todo_web_api.Interface
{
    public interface ICloudinaryRespository
    {
        Task<List<CloudinaryImage>> GetCloudinary(AppUser user);
        Task<CloudinaryImage> CreateAsync(CloudinaryImage CI);

        Task<todo_web_api.Models.UploadResult> CreateAsyncPdf(todo_web_api.Models.UploadResult CI);

        Task<List<todo_web_api.Models.UploadResult>> GetPdfs(AppUser user);

    }
}