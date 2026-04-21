using TaskTracker.Models;

namespace TaskTracker.Repositories;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);
    Task DeleteAsync(int id);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<IEnumerable<TaskItem>> SearchAsync(string term);
    Task UpdateAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> FilterAsync(string status, string sortOrder);
    Task<TaskItem?> GetByIdAsync(int id);
    Task ToggleStatusAsync(int id);
    Task<bool> ExistsAsync(int id);
}
