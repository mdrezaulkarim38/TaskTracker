using TaskTracker.Models;

namespace TaskTracker.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(int id);
    Task ToggleStatusAsync(int id);
    Task<IEnumerable<TaskItem>> SearchAsync(string term);
    Task<IEnumerable<TaskItem>> FilterAsync(string status, string sortOrder);

    Task<IEnumerable<TaskItem>> GetFilteredTasksAsync(string? search, string? status, string? sortOrder);
    Task<IEnumerable<TaskItem>> GetFilteredTasksPagedAsync(string? search, string? status, string? sortOrder, int page, int pageSize);
    Task<int> GetFilteredTasksCountAsync(string? search, string? status);
}