using System.CommandLine;
using Default.Models;
using System.Text.Json;
using Default.Commands;
using Default.Config;

namespace Default
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Recover all the data saved in local
            var UserConfig = UserConfiguration.RetrieveUserConfig();
            var UserTasksRetrieved = UserTasks.RetrieveTasks();

            //Create roo comman
            var RootCommand = new RootCommand("Default multi-tool");

            //Map all the commands to the main root
            TasksCommands.AddToRoot(RootCommand);
            Local.AddToRoot(RootCommand);
            SystemCommands.AddToRoot(RootCommand);


            RootCommand.Parse(args).Invoke();
        }
    }
}
