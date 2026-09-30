using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

public class TaskFilterRequest
{
    public TaskItemStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Search { get; set; }
}