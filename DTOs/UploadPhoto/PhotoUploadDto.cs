using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;


namespace todo_web_api.Dtos
{
    public class PhotoUploadDto

    {
        [Required]
        [FromForm(Name = "file")] // Accept lowercase key too

        public required IFormFile File { get; set; }
    }

}
