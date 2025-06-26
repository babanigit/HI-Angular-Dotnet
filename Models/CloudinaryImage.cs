


namespace todo_web_api.Models;

public class CloudinaryImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }

}