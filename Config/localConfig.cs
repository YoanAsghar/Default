using System.Text.Json;
using System.IO;

namespace Default.Config
{
    public class UserConfiguration
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;


        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DefaultTool",
            "localConfig.json"
        );

        public static UserConfiguration RetrieveUserConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
                string json = JsonSerializer.Serialize(new UserConfiguration(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
                return new UserConfiguration();
            }

            string UserDataString = File.ReadAllText(ConfigPath);
            UserConfiguration RetrievedUserData = JsonSerializer.Deserialize<UserConfiguration>(UserDataString) ?? new UserConfiguration();

            return RetrievedUserData;
        }

        public static void EditUserConfig(string username, string email)
        {
            var UserConfig = RetrieveUserConfig();
            //In case the user only passes an username, it edits the username
            if (!string.IsNullOrEmpty(username)) UserConfig.UserName = username;
            if (!string.IsNullOrEmpty(email)) UserConfig.Email = email;

            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(UserConfig, new JsonSerializerOptions { WriteIndented = true }));
            return;
        }
    }
}
