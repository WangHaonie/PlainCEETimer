using Newtonsoft.Json;

namespace PlainCEETimer.Modules
{
    public static class Json
    {
        public static T Deserialize<T>(string json)
            where T : class, new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json) ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
