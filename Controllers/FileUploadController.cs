using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using todo_web_api.Dtos;
using todo_web_api.Interface;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadController> _logger;

    private readonly IPhotoService _photoService;

    public FileUploadController(IWebHostEnvironment environment, ILogger<FileUploadController> logger, IPhotoService photoService)
    {
        _environment = environment;
        _logger = logger;
        _photoService = photoService;

    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Invalid file type. Only image files are allowed.");

            // max 5mb
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File size exceeds 5MB limit");

            // Create unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";

            // Define upload path
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "assets", "images");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            // Return file info
            var fileUrl = $"/assets/images/{fileName}";

            return Ok(new
            {
                message = "File uploaded successfully",
                fileName = fileName,
                originalFileName = file.FileName,
                fileSize = file.Length,
                fileUrl = fileUrl,
                uploadedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, "Internal server error occurred while uploading file");
        }
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultipleImages(List<IFormFile> files)
    {
        try
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files selected");

            var uploadResults = new List<object>();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    uploadResults.Add(new
                    {
                        originalFileName = file.FileName,
                        success = false,
                        error = "Invalid file type"
                    });
                    continue;
                }

                if (file.Length > 5 * 1024 * 1024)
                {
                    uploadResults.Add(new
                    {
                        originalFileName = file.FileName,
                        success = false,
                        error = "File size exceeds 5MB limit"
                    });
                    continue;
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "assets", "images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);


                uploadResults.Add(new
                {
                    originalFileName = file.FileName,
                    fileName = fileName,
                    fileSize = file.Length,
                    fileUrl = $"/assets/images/{fileName}",
                    success = true
                });
            }

            return Ok(new
            {
                message = "Files processed",
                results = uploadResults,
                totalFiles = files.Count,
                successfulUploads = uploadResults.Count(r => (bool)r.GetType().GetProperty("success")?.GetValue(r))
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading multiple files");
            return StatusCode(500, "Internal server error occurred while uploading files");
        }
    }

    [HttpGet("list")]
    public IActionResult ListUploadedImages()
    {
        try
        {
            _logger.LogTrace("Very detailed info");     // Most verbose
            _logger.LogDebug("Debug info");             // Development debugging
            _logger.LogInformation("General info");     // General information
            _logger.LogWarning("Warning message");      // Potential issues
            _logger.LogError("Error occurred");         // Errors
            _logger.LogCritical("Critical error");      // Critical failures

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "assets", "images");

            if (!Directory.Exists(uploadsFolder))
                return Ok(new { images = new List<object>() });

            var imageFiles = Directory.GetFiles(uploadsFolder)
                .Where(file => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }
                    .Contains(Path.GetExtension(file).ToLowerInvariant()))
                .Select(file => new
                {
                    fileName = Path.GetFileName(file),
                    fileUrl = $"/assets/images/{Path.GetFileName(file)}",
                    fileSize = new FileInfo(file).Length,
                    createdAt = new FileInfo(file).CreationTime
                })
                .OrderByDescending(f => f.createdAt)
                .ToList();

            return Ok(new { images = imageFiles });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing images");
            return StatusCode(500, "Internal server error occurred while listing images");
        }
    }

    [HttpPost("cloudinary")]
    public async Task<IActionResult> usingCloudinaryAsync([FromForm] PhotoUploadDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        foreach (var kvp in ModelState)
        {
            Console.WriteLine($"Key: {kvp.Key}");
            foreach (var error in kvp.Value.Errors)
            {
                Console.WriteLine($"  Error: {error.ErrorMessage}");
            }
        }


        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("No file uploaded.");

        var fileName = dto.File.FileName;

        return Ok(new
        {
            message = "Received file successfully",
            fileName,
            fileSize = dto.File.Length
        });
    }


    [HttpPost("cloudinary-direct")]
    public async Task<IActionResult> UploadDirect([FromForm] IFormFile file)
    {
        if (file == null)
            return BadRequest("No file uploaded.");

        var photoResult = await _photoService.AddPhotoAsync(file);

        return Ok(new
        {
            photoResult,
            message = "Upload succeeded",
            fileName = file.FileName
        });
    }

    [HttpGet("cloudinary-images")]
    public async Task<IActionResult> GetAllCloudinaryImages()
    {
        var images = await _photoService.GetAllPhotosAsync();
        return Ok(new
        {
            count = images.Count,
            images
        });
    }


}