using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Http;

namespace PlainCEETimer.Modules.Update;

internal static class AppUpdate
{
    private class UpdateUrlBuilder(bool action, UpdateSource src)
    {
        private readonly bool isgh = src is UpdateSource.GitHubCI or UpdateSource.GitHubStable;
        private readonly bool isci = src is UpdateSource.GitHubCI or UpdateSource.GiteeCI;

        internal string Url
        {
            get
            {
                var sb = new StringBuilder(120)
                    .Append("https://")
                    .Append(isgh ? "github.com" : "gitee.com")
                    .Append('/')
                    .Append(isgh ? "WangHaonie/PlainCEETimerStatic" : "WangHaonie/CEETimerCSharpWinForms")
                    .Append("/raw/main");

                if (action)
                {
                    sb.Append("/download")
                      .Append(isci ? "/ci/" : "/")
                      .Append("CEETimerCSharpWinForms_{0}_x64_Setup.exe");
                }
                else
                {
                    sb.Append("/api/")
                      .Append(isci ? "ci.json" : "github.json");
                }

                return sb.ToString();
            }
        }
    }

    public static async Task<AppUpdateInfo> FetchAsync(UpdateSource src, CancellationToken cancellationToken)
    {
        var res = await HttpService.GetStringAsync(GetUpdateApi(src), cancellationToken).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<AppUpdateInfo>(res).SetUrl(GetDownloadUrl(src));
    }

    public static string GetUpdateApi(UpdateSource src)
    {
        return new UpdateUrlBuilder(false, src).Url;
    }

    public static string GetDownloadUrl(UpdateSource src)
    {
        return new UpdateUrlBuilder(true, src).Url;
    }
}
