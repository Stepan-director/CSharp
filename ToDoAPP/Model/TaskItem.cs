// Models/TaskItem.cs
namespace ToDoAPP.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public DateTime? DueDate { get; set; }   // nullable, соответствует Java LocalDateTime
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}