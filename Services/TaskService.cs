
using TaskManager.Entities;
using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseEntity>> GetAllTasksAsync();
    Task<TaskResponseEntity> GetTaskByIdAsync(int id);
    Task AddTaskAsync(CreateTaskEntity taskEntity);
    Task UpdateTaskAsync(int id, CreateTaskEntity taskEntity);
    Task DeleteTaskAsync(int id);
}

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskResponseEntity>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllTasksAsync();
        return tasks.Select(t => new TaskResponseEntity
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            DueDate = t.DueDate,
            IsCompleted = t.IsCompleted,
            CategoryName = t.Category.Name
        });
    }

    public async Task<TaskResponseEntity> GetTaskByIdAsync(int id)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        if (task == null) return null;

        return new TaskResponseEntity
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            CategoryName = task.Category.Name
        };
    }

    public async Task AddTaskAsync(CreateTaskEntity taskEntity)
    {
        var task = new ITSTask
        {
            Title = taskEntity.Title,
            Description = taskEntity.Description,
            DueDate = taskEntity.DueDate,
            CategoryId = taskEntity.CategoryId
        };
        await _taskRepository.AddTaskAsync(task);
    }

    public async Task UpdateTaskAsync(int id, CreateTaskEntity taskEntity)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        if (task == null) return;

        task.Title = taskEntity.Title;
        task.Description = taskEntity.Description;
        task.DueDate = taskEntity.DueDate;
        task.CategoryId = taskEntity.CategoryId;

        await _taskRepository.UpdateTaskAsync(task);
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _taskRepository.DeleteTaskAsync(id);
    }
}
