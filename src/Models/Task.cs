namespace Default.Models
{
    public enum Priorit
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
    public class Task
    {
        int Id { get; set; }
        string Name { get; set; } = string.Empty;
        string Description { get; set; } = string.Empty;
        DateTime CreationDate { get; set; } = DateTime.Now;
        DateTime DueDate { get; set; }
        int Priority { get; set; }
    }
}
