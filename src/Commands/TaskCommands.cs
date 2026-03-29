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

            root.Subcommands.Add(TaskCommand);
        }

        public static Command AddTaskCommand()
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
                    Description = description,
                    DueDate = dueDate,
                    Priority = priority
                };

                UserTasks.SaveTasksLocally(TaskToSave);
            });

            return taskAddCommand;
        }
    }
}
