using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using todo_web_api.Dtos;
using todo_web_api.Extensions;
using todo_web_api.Interface;
using todo_web_api.Models;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadController> _logger;

    private readonly IPhotoService _photoService;

    private readonly ICloudinaryRespository _ClouRepo;

    public FileUploadController(ICloudinaryRespository ClouRepo, UserManager<AppUser> userManager, IWebHostEnvironment environment, ILogger<FileUploadController> logger, IPhotoService photoService)
    {
        _userManager = userManager;
        _environment = environment;
        _logger = logger;
        _photoService = photoService;

        _ClouRepo = ClouRepo;

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


    // cloudinary images
    [Authorize]
    [HttpPost("cloudinary-direct")]
    public async Task<IActionResult> UploadDirect([FromForm] IFormFile file)
    {

        if (file == null)
            return BadRequest("No file uploaded.");

        var photoResult = await _photoService.AddPhotoAsync(file);

        // get the username by token
        var username = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(username);
        if (appUser == null)
            return Unauthorized("User not found");

        var clouModel = new CloudinaryImage
        {
            ImageUrl = photoResult.Url.ToString(),
            AppUserId = appUser.Id
        };

        await _ClouRepo.CreateAsync(clouModel);

        if (clouModel == null)
            return StatusCode(500, "Could not create");
        else
            // return CreatedAtAction(nameof(AddTodo), new { id = todoModel.Id }, todoModel);

            return Ok(new
            {
                clouModel,
                photoResult,
                message = "Upload succeeded",
                fileName = file.FileName
            });
    }

    [Authorize]
    [HttpGet("cloudinary-images")]
    public async Task<IActionResult> GetCloudinaryimages()
    {
        var username = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(username);
        var cloudinaryImg = await _ClouRepo.GetCloudinary(appUser);

        return Ok(cloudinaryImg);
    }


    // cloudinary upload raw files ,pdf, zip

    [Authorize]
    [HttpPost("upload-pdf")]
    public async Task<IActionResult> UploadPdf([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // Optional: Validate extension
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".pdf")
            return BadRequest("Only PDF files are allowed.");

        // Upload to Cloudinary
        var uploadResult = await _photoService.AddPdfAsync(file);

        var username = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(username);
        Console.WriteLine($" the user is:- {appUser} ");
        if (appUser == null)
            return Unauthorized("User not found.");

        if (uploadResult == null || uploadResult.Url == null || uploadResult.ResourceType == null)
            return StatusCode(500, "Cloudinary upload failed or returned null values.");

        var pdfRecord = new todo_web_api.Models.UploadResult
        {
            Url = uploadResult.Url.ToString(),
            ResourceType = uploadResult.ResourceType,
            AppUserId = appUser.Id
        };
        await _ClouRepo.CreateAsyncPdf(pdfRecord);

        return Ok(new
        {
            pdfRecord.Id,
            url = pdfRecord.Url,
            resourceType = pdfRecord.ResourceType,
            pdfRecord.AppUser
        });
    }


    [Authorize]
    [HttpGet("get-pdfs")]
    public async Task<IActionResult> GetCloudinaryPDF()
    {
        var username = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(username);
        var Clou_Pdf = await _ClouRepo.GetPdfs(appUser);

        return Ok(Clou_Pdf);
    }
}