namespace TaskManager.Entities;

public class CreateTaskEntity 
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public int CategoryId { get; set; }
}
