using System;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.Update;

internal class AppUpdateInfo
{
    [JsonProperty("name")]
    public Version Version { get; set; }

    [JsonProperty("published_at")]
    public DateTime ReleaseDate { get; set; }

    [JsonProperty("body")]
    public string Changelog { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("commit")]
    public string Commit { get; set; }

    internal string Url => url;

    private string url;

    internal AppUpdateInfo SetUrl(string format)
    {
        url = string.Format(format, Version.ToString());
        return this;
    }
}