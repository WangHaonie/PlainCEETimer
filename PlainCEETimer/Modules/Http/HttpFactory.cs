using System.Net.Http;

namespace PlainCEETimer.Modules.Http
{
    public static class HttpFactory
    {
        public static HttpClient Instance { get; }

        static HttpFactory()
        {
            Instance = new();
            Instance.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
        }
    }
}
