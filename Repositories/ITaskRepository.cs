using TaskTracker.Models;

namespace TaskTracker.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<IEnumerable<TaskItem>> SearchAsync(string term);
    Task<IEnumerable<TaskItem>> FilterAsync(string status, string sortOrder);
    Task<TaskItem?> GetByIdAsync(int id);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(int id);
    Task ToggleStatusAsync(int id);
    Task<bool> ExistsAsync(int id);
}
