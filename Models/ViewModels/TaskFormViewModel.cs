using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.ViewModels;

public class TaskFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Due Date")]
    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "Priority is required")]
    [Display(Name = "Priority")]
    public Priority Priority { get; set; } = Priority.Low;

    [Display(Name = "Is Completed")]
    public bool IsCompleted { get; set; } = false;
}
