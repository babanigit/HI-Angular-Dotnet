

namespace todo_web_api.Models;
public class UploadResult
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string ResourceType { get; set; }
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}

