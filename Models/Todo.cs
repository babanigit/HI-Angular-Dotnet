using todo_web_api.Models;
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Status { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsImportant { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}
