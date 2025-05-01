using Newtonsoft.Json;
using System.IO;

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
                Formatting = Formatting.None,
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
            try
            {
                return JsonConvert.DeserializeObject<ConfigObject>(File.ReadAllText(App.ConfigFilePath));
            }
            catch
            {
                return new();
            }
        }
    }
}
