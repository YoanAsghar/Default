using System.IO;
using System.Text.Json;

namespace Default.Models
{
    public enum TasksPriority
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
    public class UserTasks
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateOnly? DueDate { get; set; }
        public TasksPriority Priority { get; set; }

        //Route for the tasks
        private static readonly string TasksPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DefaultTool",
                    "UserTasks.json"
                );

        //List of all the tasks
        public static List<UserTasks> RetrieveTasks()
        {
            if (!File.Exists(TasksPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TasksPath)!);
                File.WriteAllText(TasksPath, JsonSerializer.Serialize(new List<UserTasks>(), new JsonSerializerOptions { WriteIndented = true }));
            }

            var json = File.ReadAllText(TasksPath);
            List<UserTasks> UserTasks = JsonSerializer.Deserialize<List<UserTasks>>(json) ?? new List<UserTasks>();

            return UserTasks;
        }

        public static void SaveTasksLocally(UserTasks taskToSave)
        {
            List<UserTasks> UserTasksRetrieved = RetrieveTasks();
            taskToSave.Id = UserTasksRetrieved.Count == 0 ? 1 : UserTasksRetrieved.Last().Id + 1;
            UserTasksRetrieved.Add(taskToSave);

            var json = JsonSerializer.Serialize(UserTasksRetrieved, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(TasksPath, json);
        }

        public static void SaveTasksLocally(List<UserTasks> tasks)
        {
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TasksPath, json);
        }
    }
}
