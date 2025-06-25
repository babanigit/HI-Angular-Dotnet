// Dtos/UpdateTodoDto.cs
namespace todo_web_api.Dtos
{
    public class UpdateTodoDto
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsImportant { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
