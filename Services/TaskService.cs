using TaskTracker.Models;
using TaskTracker.Models.ViewModels;
using TaskTracker.Repositories;

namespace TaskTracker.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<TaskListViewModel> GetTaskListAsync(string? search, string? status, string? sort, int page = 1, int pageSize = 5)
    {
        var tasks = await _taskRepository.GetFilteredTasksPagedAsync(search, status, sort, page, pageSize);
        var totalItems = await _taskRepository.GetFilteredTasksCountAsync(search, status);
        var allTasks = await _taskRepository.GetAllAsync();

        return new TaskListViewModel
        {
            Tasks = tasks,
            SearchTerm = search,
            FilterStatus = status ?? "",
            SortOrder = sort ?? "",
            TotalCount = allTasks.Count(),
            CompletedCount = allTasks.Count(t => t.IsCompleted),
            PendingCount = allTasks.Count(t => !t.IsCompleted),

            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<TaskFormViewModel?> GetTaskForEditAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) return null;

        return new TaskFormViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Priority = task.Priority,
            IsCompleted = task.IsCompleted
        };
    }

    public async Task CreateTaskAsync(TaskFormViewModel model)
    {
        var task = new TaskItem
        {
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Priority = model.Priority,
            IsCompleted = model.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Creating task: {Title}", task.Title);
        await _taskRepository.AddAsync(task);
    }

    public async Task UpdateTaskAsync(TaskFormViewModel model)
    {
        var task = await _taskRepository.GetByIdAsync(model.Id);
        if (task == null) return;

        task.Title = model.Title;
        task.Description = model.Description;
        task.DueDate = model.DueDate;
        task.Priority = model.Priority;
        task.IsCompleted = model.IsCompleted;

        _logger.LogInformation("Updating task ID: {Id}", task.Id);
        await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(int id)
    {
        _logger.LogInformation("Deleting task ID: {Id}", id);
        await _taskRepository.DeleteAsync(id);
    }

    public async Task ToggleStatusAsync(int id)
    {
        _logger.LogInformation("Toggling status for task ID: {Id}", id);
        await _taskRepository.ToggleStatusAsync(id);
    }

    public async Task<IEnumerable<TaskItem>> SearchTasksAsync(string term)
    {
        return await _taskRepository.SearchAsync(term);
    }
}