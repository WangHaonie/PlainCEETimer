using System.IO;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.Configuration
{
    public static class ConfigHandler
    {
        private static readonly JsonSerializerSettings Settings;

        static ConfigHandler()
        {
            Settings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public static void Save()
        {
            File.WriteAllText(App.ConfigFilePath, JsonConvert.SerializeObject(App.AppConfig, Settings));
        }

        public static ConfigObject Read()
        {
            return Json.Deserialize<ConfigObject>(File.ReadAllText(App.ConfigFilePath));
        }
    }
}
