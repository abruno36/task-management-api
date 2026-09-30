using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1,
        ErrorMessage = "Title must contain between 1 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000,
        ErrorMessage = "Description must contain at most 1000 characters.")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [EnumDataType(typeof(TaskItemStatus),
        ErrorMessage = "Status must be Pending, InProgress or Completed.")]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
}