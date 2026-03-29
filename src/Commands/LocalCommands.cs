using System.CommandLine;
using Default.Config;

namespace Default.Commands
{
    public class Local
    {
        public static void AddToRoot(RootCommand root)
        {
            var LocalCommand = new Command("local", "Manages local configuration");

            LocalCommand.Add(ConfigureUser());

            root.Subcommands.Add(LocalCommand);
        }

        public static Command ConfigureUser()
        {
            //Creates config command
            var ConfigureUserCommand = new Command("config");

            //Creates the arguments for the config command
            var UserNameArgument = new Option<string>("--username");
            var EmailArgument = new Option<string>("--email");

            //Adds arguments to the command
            ConfigureUserCommand.Options.Add(UserNameArgument);
            ConfigureUserCommand.Options.Add(EmailArgument);

            ConfigureUserCommand.SetAction((ParseResult) =>
            {
                var UserName = ParseResult?.GetValue(UserNameArgument);
                var Email = ParseResult?.GetValue(EmailArgument);

                UserConfiguration.EditUserConfig(UserName, Email);
            });

            return ConfigureUserCommand;
        }
    }
}
