using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        try
        {
            return await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
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
            return await _context.Tasks.FindAsync(id);
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
            _context.Tasks.Update(task);
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
            var task = await GetByIdAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Task deleted successfully: {TaskTitle} (ID: {TaskId})", task.Title, id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent task with ID {TaskId}", id);
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
            var task = await GetByIdAsync(id);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Task status toggled successfully: {TaskTitle} (ID: {TaskId}) - Completed: {IsCompleted}",
                    task.Title, id, task.IsCompleted);
            }
            else
            {
                _logger.LogWarning("Attempted to toggle status of non-existent task with ID {TaskId}", id);
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
            return await _context.Tasks.AnyAsync(t => t.Id == id);
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
            if (string.IsNullOrWhiteSpace(term))
                return await GetAllAsync();

            return await _context.Tasks
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
            var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "completed")
                    query = query.Where(t => t.IsCompleted);
                else if (status.ToLower() == "pending")
                    query = query.Where(t => !t.IsCompleted);
            }

            if (sortOrder?.ToLower() == "duedate")
            {
                query = query.OrderBy(t => t.DueDate);
            }
            else if (sortOrder?.ToLower() == "priority")
            {
                query = query.OrderBy(t => t.Priority);
            }
            else
            {
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering tasks with status: {Status}, sortOrder: {SortOrder}", status, sortOrder);
            throw;
        }
    }

    
}