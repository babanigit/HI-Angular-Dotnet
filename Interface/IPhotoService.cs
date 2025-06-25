

using CloudinaryDotNet.Actions;

namespace todo_web_api.Interface
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicUrl);

        Task<List<string>> GetAllPhotosAsync();

    }
}