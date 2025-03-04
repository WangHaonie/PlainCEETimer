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

        public static int GetAutoSwitchInterval(int Index) => Index switch
        {
            1 => 15_000, // 15 s
            2 => 30_000, // 30 s
            3 => 45_000, // 45 s
            4 => 60_000, // 1 min
            5 => 120_000, // 2 min
            6 => 180_000, // 3 min
            7 => 300_000, // 5 min
            8 => 600_000, // 10 min
            9 => 900_000, // 15 min
            10 => 1_800_000, // 30 min
            11 => 2_700_000, // 45 min
            12 => 3_600_000, // 1 h
            _ => 10_000 // 10 s
        };
    }
}
