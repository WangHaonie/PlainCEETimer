using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Http
{
    public static class HttpService
    {
        private static readonly HttpClient Client;

        static HttpService()
        {
            Client = new();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
        }

        public static Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return Client.GetAsync(requestUri, completionOption, cancellationToken);
        }

        public static Task<string> GetStringAsync(string requestUri)
        {
            return Client.GetStringAsync(requestUri);
        }
    }
}
