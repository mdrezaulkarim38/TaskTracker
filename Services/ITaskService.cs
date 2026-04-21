using TaskTracker.Models;
using TaskTracker.Models.ViewModels;

namespace TaskTracker.Services;

public interface ITaskService
{
    Task<TaskListViewModel> GetTaskListAsync(string? search, string? status, string? sort);
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task<TaskFormViewModel?> GetTaskForEditAsync(int id);
    Task CreateTaskAsync(TaskFormViewModel model);
    Task UpdateTaskAsync(TaskFormViewModel model);
    Task DeleteTaskAsync(int id);
    Task ToggleStatusAsync(int id);
    Task<IEnumerable<TaskItem>> SearchTasksAsync(string term);
}