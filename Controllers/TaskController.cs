using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<IActionResult> Index(string? search, string? status, string? sort)
    {
        var model = await _taskService.GetTaskListAsync(search, status, sort);
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
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            await _taskService.DeleteTaskAsync(id);
            TempData["Success"] = "Task deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task ID: {Id}", id);
            TempData["Error"] = "Something went wrong. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            var all = await _taskService.GetTaskListAsync(null, "all", "desc");
            return PartialView("_TaskTablePartial", all.Tasks);
        }

        var tasks = await _taskService.SearchTasksAsync(term);
        return PartialView("_TaskTablePartial", tasks);
    }
}
