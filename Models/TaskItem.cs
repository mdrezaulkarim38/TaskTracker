using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class TaskItem {
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}