using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Http;

public static class HttpService
{
    private static readonly HttpClient Client;

    static HttpService()
    {
        Client = new();
        Client.DefaultRequestHeaders.UserAgent.ParseAdd($"{App.AppNameEng}/{App.AppVersion} (Windows NT; Win64; x64)");
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
