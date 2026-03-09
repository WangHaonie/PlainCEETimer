using System;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.Update;

internal class AppUpdateInfo
{
    [JsonProperty("name")]
    public Version Version { get; init; }

    [JsonProperty("published_at")]
    public DateTime ReleaseDate { get; init; }

    [JsonProperty("body")]
    public string Changelog { get; init; }

    [JsonProperty("size")]
    public long Size { get; init; }

    [JsonProperty("commit")]
    public string Commit { get; init; }

    internal string Url => url;

    private string url;

    internal AppUpdateInfo SetUrl(string format)
    {
        url = string.Format(format, Version.ToString());
        return this;
    }
}