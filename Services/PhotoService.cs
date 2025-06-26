

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using todo_web_api.Helpers;
using todo_web_api.Interface;

namespace todo_web_api.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloundinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
              config.Value.CloudName,
              config.Value.ApiKey,
              config.Value.ApiSecret
              );

            // Log or Debug the values to confirm they're being passed correctly
            Console.WriteLine($"CloudName: {config.Value.CloudName}");
            Console.WriteLine($"ApiKey: {config.Value.ApiKey}");
            Console.WriteLine($"ApiSecret: {config.Value.ApiSecret}");


            _cloundinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {

                // var fileExtension = Path.GetExtension(file.FileName).ToLower();

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloundinary.UploadAsync(uploadParams);


            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicUrl)
        {
            var publicId = publicUrl.Split('/').Last().Split('.')[0];
            var deleteParams = new DeletionParams(publicId);
            return await _cloundinary.DestroyAsync(deleteParams);
        }

        public async Task<List<string>> GetAllPhotosAsync()
        {
            var listParams = new ListResourcesParams()
            {
                MaxResults = 100, // You can adjust this
                Type = "upload",
                ResourceType = ResourceType.Image
            };

            var result = await _cloundinary.ListResourcesAsync(listParams);

            var imageUrls = result.Resources
                .Select(r => r.SecureUrl.ToString())
                .ToList();

            return imageUrls;
        }


        public async Task<RawUploadResult> AddPdfAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName)
                };

                var uploadResult = await _cloundinary.UploadAsync(uploadParams);
                return uploadResult;
            }

            return null!;
        }





    }
}