namespace TaskManager.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<ITSTask> Tasks { get; set; }
}

