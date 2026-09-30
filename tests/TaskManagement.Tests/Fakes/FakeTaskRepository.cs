using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Tests.Fakes;

public class FakeTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = new();

    public Task<IEnumerable<TaskItem>> GetAllAsync(
        TaskItemStatus? status = null,
        DateTime? dueDate = null,
        string? search = null)
    {
        IEnumerable<TaskItem> query = _tasks;

        if (status.HasValue)
        {
            query = query.Where(task =>
                task.Status == status.Value);
        }

        if (dueDate.HasValue)
        {
            query = query.Where(task =>
                task.DueDate.HasValue &&
                task.DueDate.Value.Date == dueDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(task =>
                task.Title.Contains(search) ||
                (task.Description != null &&
                 task.Description.Contains(search)));
        }

        return Task.FromResult(query);
    }

    public Task<TaskItem?> GetByIdAsync(Guid id)
    {
        var task = _tasks.FirstOrDefault(task => task.Id == id);

        return Task.FromResult(task);
    }

    public Task<TaskItem> AddAsync(TaskItem taskItem)
    {
        _tasks.Add(taskItem);

        return Task.FromResult(taskItem);
    }

    public Task UpdateAsync(TaskItem taskItem)
    {
        var existingTask = _tasks.FirstOrDefault(
            task => task.Id == taskItem.Id);

        if (existingTask is not null)
        {
            existingTask.Title = taskItem.Title;
            existingTask.Description = taskItem.Description;
            existingTask.DueDate = taskItem.DueDate;
            existingTask.Status = taskItem.Status;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(TaskItem taskItem)
    {
        _tasks.Remove(taskItem);

        return Task.CompletedTask;
    }
}