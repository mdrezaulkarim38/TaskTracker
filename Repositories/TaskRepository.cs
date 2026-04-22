using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskRepository> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskRepository(
        ApplicationDbContext context,
        ILogger<TaskRepository> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not logged in");

        return userId;
    }

    private IQueryable<TaskItem> BuildFilteredQuery(string? search, string? status)
    {
        var userId = GetCurrentUserId();
        var query = _context.Tasks.Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.Title.Contains(search) ||
                (t.Description != null && t.Description.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status.ToLower() == "completed")
                query = query.Where(t => t.IsCompleted);
            else if (status.ToLower() == "pending")
                query = query.Where(t => !t.IsCompleted);
        }

        return query;
    }

    private IQueryable<TaskItem> ApplySorting(IQueryable<TaskItem> query, string? sortOrder)
    {
        if (sortOrder?.ToLower() == "duedate")
        {
            return query.OrderBy(t => t.DueDate ?? DateTime.MaxValue);
        }
        else if (sortOrder?.ToLower() == "priority")
        {
            return query.OrderByDescending(t => t.Priority);
        }
        else
        {
            return query.OrderByDescending(t => t.CreatedAt);
        }
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tasks");
            throw;
        }
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task with ID {TaskId}", id);
            throw;
        }
    }

    public async Task AddAsync(TaskItem task)
    {
        try
        {
            task.UserId = GetCurrentUserId();
            task.CreatedAt = DateTime.UtcNow;

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Task added successfully: {TaskTitle} (ID: {TaskId})", task.Title, task.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding new task: {TaskTitle}", task.Title);
            throw;
        }
    }

    public async Task UpdateAsync(TaskItem task)
    {
        try
        {
            var userId = GetCurrentUserId();
            var existingTask = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id && t.UserId == userId);

            if (existingTask == null)
            {
                _logger.LogWarning("Attempted to update non-existent or unauthorized task with ID {TaskId}", task.Id);
                throw new UnauthorizedAccessException("Task not found or access denied");
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Priority = task.Priority;
            existingTask.IsCompleted = task.IsCompleted;

            _context.Tasks.Update(existingTask);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Task updated successfully: {TaskTitle} (ID: {TaskId})", task.Title, task.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task with ID {TaskId}", task.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Task deleted successfully: {TaskTitle} (ID: {TaskId})", task.Title, id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent or unauthorized task with ID {TaskId}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task with ID {TaskId}", id);
            throw;
        }
    }

    public async Task ToggleStatusAsync(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                await _context.SaveChangesAsync();
                _logger.LogInformation(
                    "Task status toggled successfully: {TaskTitle} (ID: {TaskId}) - Completed: {IsCompleted}",
                    task.Title, id, task.IsCompleted);
            }
            else
            {
                _logger.LogWarning("Attempted to toggle status of non-existent or unauthorized task with ID {TaskId}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling status for task with ID {TaskId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            return await _context.Tasks.AnyAsync(t => t.Id == id && t.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of task with ID {TaskId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TaskItem>> SearchAsync(string term)
    {
        try
        {
            var userId = GetCurrentUserId();
            var query = _context.Tasks.Where(t => t.UserId == userId);

            if (string.IsNullOrWhiteSpace(term))
                return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

            return await query
                .Where(t => t.Title.Contains(term) ||
                            (t.Description != null && t.Description.Contains(term)))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tasks with term: {Term}", term);
            throw;
        }
    }

    public async Task<IEnumerable<TaskItem>> FilterAsync(string status, string sortOrder)
    {
        try
        {
            var query = BuildFilteredQuery(null, status);
            query = ApplySorting(query, sortOrder);

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering tasks with status: {Status}, sortOrder: {SortOrder}", status, sortOrder);
            throw;
        }
    }

    public async Task<IEnumerable<TaskItem>> GetFilteredTasksAsync(string? search, string? status, string? sortOrder)
    {
        try
        {
            var query = BuildFilteredQuery(search, status);
            query = ApplySorting(query, sortOrder);

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering tasks with search: {Search}, status: {Status}, sortOrder: {SortOrder}",
                search, status, sortOrder);
            throw;
        }
    }

    public async Task<IEnumerable<TaskItem>> GetFilteredTasksPagedAsync(string? search, string? status, string? sortOrder, int page, int pageSize)
    {
        try
        {
            var query = BuildFilteredQuery(search, status);
            query = ApplySorting(query, sortOrder);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged tasks with search: {Search}, status: {Status}, sortOrder: {SortOrder}",
                search, status, sortOrder);
            throw;
        }
    }

    public async Task<int> GetFilteredTasksCountAsync(string? search, string? status)
    {
        try
        {
            var query = BuildFilteredQuery(search, status);
            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting filtered tasks with search: {Search}, status: {Status}",
                search, status);
            throw;
        }
    }
}