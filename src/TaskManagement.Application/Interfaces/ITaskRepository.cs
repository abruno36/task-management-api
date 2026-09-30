using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(
        TaskItemStatus? status = null,
        DateTime? dueDate = null,
        string? search = null);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<TaskItem> AddAsync(TaskItem taskItem);
    Task UpdateAsync(TaskItem taskItem);
    Task DeleteAsync(TaskItem taskItem);
}