namespace TaskTracker.Models.ViewModels;

public class TaskListViewModel
{
    public IEnumerable<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public string? SearchTerm { get; set; }
    public string? FilterStatus { get; set; }
    public string? SortOrder { get; set; }

    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
    public int PendingCount { get; set; }
}
