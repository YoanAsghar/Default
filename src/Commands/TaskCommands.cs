using System.CommandLine;

namespace Default.Commands
{
    public class Tasks
    {
        public static void AddToRoot(RootCommand root)
        {
            var TaskCommand = new Command("task", "Task management");

            TaskCommand.Add(AddCommand());

            root.Subcommands.Add(TaskCommand);
        }

        public static Command AddCommand()
        {

            //Creates add command
            var taskAddCommand = new Command("add", "Creates new task");

            //Creates the arguments for the add command
            var NameArgument = new Argument<string>("name");
            var DescriptionOption = new Option<string>("-d");
            var DueDate = new Option<DateOnly>("--d-d");

            //Add the arguments to the command
            taskAddCommand.Arguments.Add(NameArgument);
            taskAddCommand.Options.Add(DescriptionOption);
            taskAddCommand.Options.Add(DueDate);

            //Logic of the command
            taskAddCommand.SetAction((ParseResult) =>
            {
                string? name = ParseResult.GetValue(NameArgument);
                string? description = ParseResult.GetValue(DescriptionOption);
                DateOnly? dueDate = ParseResult.GetValue(DueDate);

                Console.WriteLine(name + description + dueDate);
            });

            return taskAddCommand;
        }
    }
}
