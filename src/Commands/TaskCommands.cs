using Default.Models;
using System.CommandLine;

namespace Default.Commands
{
    public class TasksCommands
    {
        public static void AddToRoot(RootCommand root)
        {
            var TaskCommand = new Command("tasks", "Task management");

            TaskCommand.Add(AddTaskCommand());
            TaskCommand.Add(ListTaskCommand());
            TaskCommand.Add(DeleteTaskCommand());

            root.Subcommands.Add(TaskCommand);
        }

        public static Command AddTaskCommand()
        {

            try
            {

                //Creates add command
                var taskAddCommand = new Command("add", "Creates new task");

                //Creates the arguments for the add command
                var NameOption = new Option<string>("--name") { Required = true };
                var DescriptionOption = new Option<string>("--description");
                var DueDate = new Option<DateOnly>("--due-date");
                var Priority = new Option<TasksPriority>("--priority") { DefaultValueFactory = _ => TasksPriority.None };

                //Add the arguments to the command
                taskAddCommand.Options.Add(NameOption);
                taskAddCommand.Options.Add(DescriptionOption);
                taskAddCommand.Options.Add(DueDate);
                taskAddCommand.Options.Add(Priority);

                //Logic of the command
                taskAddCommand.SetAction((ParseResult) =>
                    {
                        string? name = ParseResult.GetValue(NameOption);
                        string? description = ParseResult.GetValue(DescriptionOption);
                        DateOnly? dueDate = ParseResult.GetValue(DueDate);
                        TasksPriority priority = ParseResult.GetValue(Priority);

                        var TaskToSave = new UserTasks()
                        {
                            Name = name,
                            DueDate = dueDate,
                            Priority = priority
                        };

                        UserTasks.SaveTasksLocally(TaskToSave);
                        Console.WriteLine($"Created task: '{TaskToSave.Name}'");
                    });

                return taskAddCommand;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static void PrintConsoleField(string label, object? value)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{label}: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{value} ");
        }

        public static void PrintDueDate(DateOnly? dueDate)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Due Date: ");

            if (dueDate == null)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("No due date");
                return;
            }

            int daysLeft = dueDate.Value.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber;

            Console.ForegroundColor = daysLeft switch
            {
                < 0 => ConsoleColor.Red,
                < 3 => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };

            Console.WriteLine(daysLeft switch
            {
                < 0 => $"{dueDate} (Overdue by {Math.Abs(daysLeft)} days)",
                0 => $"{dueDate} (Due today!)",
                1 => $"{dueDate} (Due tomorrow!)",
                _ => $"{dueDate} ({daysLeft} days left)"
            });
        }

        public static void PrintPriority(TasksPriority priority)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Priority: ");
            Console.ForegroundColor = priority switch
            {
                TasksPriority.None => ConsoleColor.Gray,
                TasksPriority.Low => ConsoleColor.Green,
                TasksPriority.Medium => ConsoleColor.DarkYellow,
                TasksPriority.High => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };
            Console.WriteLine(priority);

            Console.ResetColor();
        }

        public static Command ListTaskCommand()
        {
            var TaskListCommand = new Command("list", "Shows tasks created");

            TaskListCommand.SetAction((ParseResult) =>
            {
                var UserTasksRetrieved = UserTasks.RetrieveTasks();

                foreach (UserTasks task in UserTasksRetrieved)
                {
                    PrintConsoleField("Id", task.Id);
                    PrintConsoleField("Name", task.Name);
                    PrintConsoleField("Creation Date", task.CreationDate);
                    PrintPriority(task.Priority);
                    PrintDueDate(task.DueDate);
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            });

            return TaskListCommand;
        }

        public static Command DeleteTaskCommand()
        {
            try
            {

                var DeleteTaskCommand = new Command("delete", "Deletes a task by id");

                //Creates argument for the id of the task to delete
                var TaskToDelete = new Argument<int>("taskId");

                DeleteTaskCommand.Arguments.Add(TaskToDelete);

                DeleteTaskCommand.SetAction((ParseResult) =>
                    {
                        var TaskId = ParseResult.GetValue(TaskToDelete);
                        var AllTasks = UserTasks.RetrieveTasks();
                        UserTasks? TaskToDeleteFromList = AllTasks.FirstOrDefault(t => t.Id == TaskId);
                        if (TaskToDeleteFromList == null)
                        {
                            Console.WriteLine("Task doesn't exist");
                            return;
                        }
                        AllTasks.Remove(TaskToDeleteFromList);
                        UserTasks.SaveTasksLocally(AllTasks);

                        Console.WriteLine($"Deleted task {TaskToDeleteFromList.Name}");
                    });

                return DeleteTaskCommand;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
