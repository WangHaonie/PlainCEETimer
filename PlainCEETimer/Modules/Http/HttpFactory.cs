using System.Net.Http;

namespace PlainCEETimer.Modules.Http
{
    public static class HttpFactory
    {
        public static HttpClient Instance { get; }

        static HttpFactory()
        {
            Instance = new();
            Instance.DefaultRequestHeaders.UserAgent.ParseAdd(App.RequestUA);
        }
    }
}
