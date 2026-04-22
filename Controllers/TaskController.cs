using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TaskTracker.Models.ViewModels;
using TaskTracker.Services;

namespace TaskTracker.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, string? status, string? sort, int page = 1)
    {
        var model = await _taskService.GetTaskListAsync(search, status, sort, page, 5);
        return View(model);
    }

    public IActionResult Create()
    {
        return View(new TaskFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _taskService.CreateTaskAsync(model);
            TempData["Success"] = "Task created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _taskService.GetTaskForEditAsync(id);
        if (model == null)
        {
            TempData["Error"] = "Task not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TaskFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var exists = await _taskService.GetTaskByIdAsync(model.Id);
            if (exists == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            await _taskService.UpdateTaskAsync(model);
            TempData["Success"] = "Task updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task ID: {Id}", model.Id);
            ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var exists = await _taskService.GetTaskByIdAsync(id);
            if (exists == null)
            {
                return Json(new { success = false, message = "Task not found." });
            }

            await _taskService.DeleteTaskAsync(id);
            return Json(new { success = true, message = "Task deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task ID: {Id}", id);
            return Json(new { success = false, message = "Something went wrong. Please try again." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return Json(new { success = false, message = "Task not found." });

            await _taskService.ToggleStatusAsync(id);
            var updated = await _taskService.GetTaskByIdAsync(id);

            return Json(new { success = true, isCompleted = updated!.IsCompleted });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling status for task ID: {Id}", id);
            return Json(new { success = false, message = "Something went wrong." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? term)
    {
        var model = await _taskService.GetTaskListAsync(term, null, null, 1, 5);
        return PartialView("_TaskTablePartial", model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv(string? search, string? status, string? sort)
    {
        try
        {
            var tasks = await _taskService.GetAllTasksForExportAsync(search, status, sort);
            if (!tasks.Any())
            {
                TempData["Warning"] = "No tasks found to export.";
                return RedirectToAction(nameof(Index));
            }

            var csv = new StringBuilder();
            var utf8WithBom = new UTF8Encoding(true);
            csv.AppendLine("\"ID\",\"Title\",\"Description\",\"Due Date\",\"Priority\",\"Status\",\"Created At\"");
            foreach (var task in tasks)
            {
                static string Escape(string? value) =>
                    string.IsNullOrEmpty(value) ? "\"\"" : "\"" + value.Replace("\"", "\"\"") + "\"";

                csv.AppendLine(string.Join(",",
                    task.Id.ToString(),
                    Escape(task.Title),
                    Escape(task.Description),
                    $"\"{task.DueDate:yyyy-MM-dd}\"",
                    Escape(task.Priority.ToString()),
                    $"\"{(task.IsCompleted ? "Completed" : "Pending")}\"",
                    $"\"{task.CreatedAt:yyyy-MM-dd HH:mm:ss}\""
                ));
            }

            var bytes = utf8WithBom.GetBytes(csv.ToString());
            var fileName = $"tasks_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv; charset=utf-8", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting tasks to CSV");
            TempData["Error"] = "Failed to export tasks. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }
}
