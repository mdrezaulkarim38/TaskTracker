using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskRepository> _logger;
    public TaskRepository(ApplicationDbContext context, ILogger<TaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> FilterAsync(string status, string sortOrder)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> SearchAsync(string term)
    {
        throw new NotImplementedException();
    }

    public Task ToggleStatusAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(TaskItem task)
    {
        throw new NotImplementedException();
    }
}
