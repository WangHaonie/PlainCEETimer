using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Http;

public static class HttpService
{
    private static readonly HttpClient client;

    static HttpService()
    {
        client = new();
        client.DefaultRequestHeaders.UserAgent.ParseAdd($"{App.AppNameEng}/{AppInfo.Version} (Windows NT; Win64; x64)");
    }

    public static Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return client.SendAsync(request);
    }

    public static Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
        return client.GetAsync(requestUri, completionOption, cancellationToken);
    }

    public static Task<string> GetStringAsync(string requestUri)
    {
        return client.GetStringAsync(requestUri);
    }
}
