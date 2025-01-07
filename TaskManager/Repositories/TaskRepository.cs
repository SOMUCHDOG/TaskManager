using TaskManager.Models;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<ITSTask>> GetAllTasksAsync();
    Task<ITSTask> GetTaskByIdAsync(int Id);
    Task AddTaskAsync(ITSTask task);
    Task UpdateTaskAsync(ITSTask task);
    Task DeleteTaskAsync(int id);
}

public class TaskRepository: ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ITSTask>> GetAllTasksAsync()
    {
        return await _context.Tasks.Include(t => t.Category).ToListAsync();
    }

    public async Task<ITSTask> GetTaskByIdAsync(int Id)
    {
         return await _context.Tasks.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == Id);
    }

    public async Task AddTaskAsync(ITSTask task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(ITSTask task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
