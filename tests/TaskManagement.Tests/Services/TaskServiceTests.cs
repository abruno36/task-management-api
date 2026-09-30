using Microsoft.Extensions.Logging.Abstractions;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Tests.Fakes;

namespace TaskManagement.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Existing task",
            Description = "Existing description",
            DueDate = new DateTime(2026, 10, 10),
            Status = TaskItemStatus.InProgress
        };

        await repository.AddAsync(taskItem);

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        // Act
        var result = await service.GetByIdAsync(taskItem.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskItem.Id, result.Id);
        Assert.Equal(taskItem.Title, result.Title);
        Assert.Equal(taskItem.Description, result.Description);
        Assert.Equal(taskItem.DueDate, result.DueDate);
        Assert.Equal(taskItem.Status, result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var id = Guid.NewGuid();

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTaskAndReturnResponse()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var request = new CreateTaskRequest
        {
            Title = "Test task",
            Description = "Test description",
            DueDate = new DateTime(2026, 10, 10),
            Status = TaskItemStatus.Pending
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.DueDate, result.DueDate);
        Assert.Equal(request.Status, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTask_WhenTaskExists()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Original title",
            Description = "Original description",
            DueDate = new DateTime(2026, 10, 10),
            Status = TaskItemStatus.Pending
        };

        await repository.AddAsync(taskItem);

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var request = new UpdateTaskRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            DueDate = new DateTime(2026, 11, 10),
            Status = TaskItemStatus.Completed
        };

        // Act
        var result = await service.UpdateAsync(
            taskItem.Id,
            request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskItem.Id, result.Id);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.DueDate, result.DueDate);
        Assert.Equal(request.Status, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var id = Guid.NewGuid();

        var request = new UpdateTaskRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            DueDate = new DateTime(2026, 11, 10),
            Status = TaskItemStatus.Completed
        };

        // Act
        var result = await service.UpdateAsync(id, request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenTaskExists()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task to delete",
            Description = "Task description",
            DueDate = new DateTime(2026, 10, 10),
            Status = TaskItemStatus.Pending
        };

        await repository.AddAsync(taskItem);

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        // Act
        var result = await service.DeleteAsync(taskItem.Id);

        // Assert
        Assert.True(result);

        var deletedTask = await repository.GetByIdAsync(taskItem.Id);

        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var id = Guid.NewGuid();

        // Act
        var result = await service.DeleteAsync(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            Status = TaskItemStatus.Pending
        });

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 2",
            Status = TaskItemStatus.Completed
        });

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var filter = new TaskFilterRequest();

        // Act
        var result = await service.GetAllAsync(filter);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByStatus()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Pending task",
            Status = TaskItemStatus.Pending
        });

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Completed task",
            Status = TaskItemStatus.Completed
        });

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var filter = new TaskFilterRequest
        {
            Status = TaskItemStatus.Pending
        };

        // Act
        var result = await service.GetAllAsync(filter);

        // Assert
        var tasks = result.ToList();

        Assert.Single(tasks);
        Assert.Equal("Pending task", tasks[0].Title);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByDueDate()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task with matching date",
            DueDate = new DateTime(2026, 10, 10, 15, 30, 0),
            Status = TaskItemStatus.Pending
        });

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task with different date",
            DueDate = new DateTime(2026, 10, 11, 10, 0, 0),
            Status = TaskItemStatus.Pending
        });

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var filter = new TaskFilterRequest
        {
            DueDate = new DateTime(2026, 10, 10)
        };

        // Act
        var result = await service.GetAllAsync(filter);

        // Assert
        var tasks = result.ToList();

        Assert.Single(tasks);
        Assert.Equal("Task with matching date", tasks[0].Title);
    }

    [Fact]
    public async Task GetAllAsync_ShouldSearchByTitleOrDescription()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Implement API",
            Description = "Backend development",
            Status = TaskItemStatus.Pending
        });

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Write documentation",
            Description = "API documentation",
            Status = TaskItemStatus.Pending
        });

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Database task",
            Description = "Database configuration",
            Status = TaskItemStatus.Pending
        });

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var filter = new TaskFilterRequest
        {
            Search = "API"
        };

        // Act
        var result = await service.GetAllAsync(filter);

        // Assert
        var tasks = result.ToList();

        Assert.Equal(2, tasks.Count);
        Assert.Contains(tasks, task => task.Title == "Implement API");
        Assert.Contains(tasks, task => task.Title == "Write documentation");
    }

    [Fact]
    public async Task GetAllAsync_ShouldApplyMultipleFilters()
    {
        // Arrange
        var repository = new FakeTaskRepository();

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Implement API",
            Description = "Backend development",
            DueDate = new DateTime(2026, 10, 10, 15, 30, 0),
            Status = TaskItemStatus.Pending
        });

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Implement API",
            Description = "Backend development",
            DueDate = new DateTime(2026, 10, 10),
            Status = TaskItemStatus.Completed
        });

        await repository.AddAsync(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Implement API",
            Description = "Backend development",
            DueDate = new DateTime(2026, 10, 11),
            Status = TaskItemStatus.Pending
        });

        var service = new TaskService(
            repository,
            NullLogger<TaskService>.Instance);

        var filter = new TaskFilterRequest
        {
            Status = TaskItemStatus.Pending,
            DueDate = new DateTime(2026, 10, 10),
            Search = "API"
        };

        // Act
        var result = await service.GetAllAsync(filter);

        // Assert
        var tasks = result.ToList();

        Assert.Single(tasks);
        Assert.Equal("Implement API", tasks[0].Title);
        Assert.Equal(TaskItemStatus.Pending, tasks[0].Status);
        Assert.Equal(
            new DateTime(2026, 10, 10, 15, 30, 0),
            tasks[0].DueDate);
    }
}