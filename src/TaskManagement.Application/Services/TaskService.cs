using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        ITaskRepository taskRepository,
        ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskResponse>> GetAllAsync(
        TaskFilterRequest filter)
    {
        var tasks = await _taskRepository.GetAllAsync(
            filter.Status,
            filter.DueDate,
            filter.Search);

        return tasks.Select(MapToResponse);
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid id)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id);

        if (taskItem is null)
            return null;

        return MapToResponse(taskItem);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request)
    {
        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Status = request.Status
        };

        var createdTask = await _taskRepository.AddAsync(taskItem);

        _logger.LogInformation(
            "Task {TaskId} created successfully.",
            createdTask.Id);

        return MapToResponse(createdTask);
    }

    public async Task<TaskResponse?> UpdateAsync(
        Guid id,
        UpdateTaskRequest request)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id);

        if (taskItem is null)
        {
            _logger.LogWarning(
                "Task {TaskId} was not found for update.",
                id);

            return null;
        }

        taskItem.Title = request.Title;
        taskItem.Description = request.Description;
        taskItem.DueDate = request.DueDate;
        taskItem.Status = request.Status;

        await _taskRepository.UpdateAsync(taskItem);

        _logger.LogInformation(
            "Task {TaskId} updated successfully.",
            taskItem.Id);

        return MapToResponse(taskItem);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id);

        if (taskItem is null)
        {
            _logger.LogWarning(
                "Task {TaskId} was not found for deletion.",
                id);

            return false;
        }

        await _taskRepository.DeleteAsync(taskItem);

        _logger.LogInformation(
            "Task {TaskId} deleted successfully.",
            taskItem.Id);

        return true;
    }

    private static TaskResponse MapToResponse(TaskItem taskItem)
    {
        return new TaskResponse
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            DueDate = taskItem.DueDate,
            Status = taskItem.Status
        };
    }
}