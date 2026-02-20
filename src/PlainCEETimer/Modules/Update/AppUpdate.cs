using System.Threading.Tasks;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Http;

namespace PlainCEETimer.Modules.Update;

internal static class AppUpdate
{
    public static async Task<AppUpdateInfo> FetchAsync(UpdateSource src)
    {
        var res = await HttpService.GetStringAsync(GetUpdateApi(src)).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<AppUpdateInfo>(res).SetUrl(GetDownloadUrl(src));
    }

    public static string GetUpdateApi(UpdateSource src) => src switch
    {
        UpdateSource.GiteeCI
            => "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/api/ci.json",
        UpdateSource.GitHubCI
            => "https://github.com/WangHaonie/PlainCEETimerStatic/raw/main/api/ci.json",
        UpdateSource.GitHubStable
            => "https://github.com/WangHaonie/PlainCEETimerStatic/raw/main/api/github.json",
        _
            => "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/api/github.json"
    };

    public static string GetDownloadUrl(UpdateSource src) => src switch
    {
        UpdateSource.GiteeCI
            => "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/download/ci/CEETimerCSharpWinForms_{0}_x64_Setup.exe",
        UpdateSource.GitHubCI
            => "https://github.com/WangHaonie/PlainCEETimerStatic/raw/main/download/ci/CEETimerCSharpWinForms_{0}_x64_Setup.exe",
        UpdateSource.GitHubStable
            => "https://github.com/WangHaonie/PlainCEETimerStatic/raw/main/download/CEETimerCSharpWinForms_{0}_x64_Setup.exe",
        _
            => "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/download/CEETimerCSharpWinForms_{0}_x64_Setup.exe"
    };

}
