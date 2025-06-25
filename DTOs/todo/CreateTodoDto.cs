public class CreateTodoDto
{
    public string Title { get; set; }
    public string Text { get; set; }

    public DateTime? DueDate { get; set; }
    public bool IsImportant { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
