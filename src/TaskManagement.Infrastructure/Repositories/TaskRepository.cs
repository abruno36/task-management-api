using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(
        TaskItemStatus? status = null,
        DateTime? dueDate = null,
        string? search = null)
    {
        var query = _context.Tasks.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(task => task.Status == status.Value);
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

        return await query.ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(task => task.Id == id);
    }

    public async Task<TaskItem> AddAsync(TaskItem taskItem)
    {
        await _context.Tasks.AddAsync(taskItem);
        await _context.SaveChangesAsync();

        return taskItem;
    }

    public async Task UpdateAsync(TaskItem taskItem)
    {
        _context.Tasks.Update(taskItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem taskItem)
    {
        _context.Tasks.Remove(taskItem);
        await _context.SaveChangesAsync();
    }
}
