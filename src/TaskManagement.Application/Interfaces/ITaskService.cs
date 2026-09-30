using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetAllAsync(TaskFilterRequest filter);
    Task<TaskResponse?> GetByIdAsync(Guid id);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request);
    Task<TaskResponse?> UpdateAsync(Guid id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(Guid id);
}
