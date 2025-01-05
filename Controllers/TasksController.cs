using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Entities;
using TaskManager.Services;

namespace TaskManager.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _logger = logger;
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        _logger.LogInformation("Fetching all tasks...");
        var tasks = await _taskService.GetAllTasksAsync();
        _logger.LogInformation("Successfully fetched {Count} tasks", tasks.Count());

        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> AddTask([FromBody] CreateTaskEntity taskEntity)
    {
        await _taskService.AddTaskAsync(taskEntity);
        return CreatedAtAction(nameof(GetTaskById), new { id = taskEntity.CategoryId }, taskEntity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] CreateTaskEntity taskEntity)
    {
        await _taskService.UpdateTaskAsync(id, taskEntity);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }
}
