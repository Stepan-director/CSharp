// Models/TaskItem.cs
namespace ToDoAPP.Models;

public class TaskItem
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public DateTime? DueDate { get; set; }   // nullable, соответствует Java LocalDateTime
}