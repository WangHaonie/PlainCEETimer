using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules.Update;

[NoConstants]
internal class Updater
{
    private class Session
    {
        public Task<AppUpdateInfo> Task;
        public bool PopupRequested;
        public bool Presented;
    }

    private class State
    {
        public Session Session;
        public AppUpdateInfo Info;
        public ulong CheckedAt;
    }

    public static Updater Instance { get; } = new();

    private DownloaderForm FormDownloader;
    private readonly object lockobj = new();
    private readonly Dictionary<UpdateSource, State> sources = [];
    private readonly SemaphoreSlim dialogSemaphore = new(1, 1);

    private const int RequestTimeoutMs = 10 * 1000;
    private const ulong ResultCacheMs = 2 * 60 * 1000;

    private Updater()
    {
        return;
    }

    public async Task CheckForUpdateAsync(bool popup, IAppWindow owner, bool isPreview = false)
    {
#if DEBUG
        if (!popup)
        {
            return;
        }
#endif
        var source = isPreview ? UpdateSource.GiteeCI : UpdateSource.GiteeStable;
        var mx = owner.MessageX;
        AppUpdateInfo info;

        try
        {
            if (TryGetCachedResult(source, out var cached))
            {
                info = cached;
            }
            else
            {
                var session = StartOrJoin(source, popup);
                info = await session.Task.ConfigureAwait(false);

                lock (lockobj)
                {
                    if (session.Presented || (!popup && session.PopupRequested))
                    {
                        return;
                    }

                    session.Presented = true;
                }
            }
        }
        catch (Exception ex)
        {
            if (popup)
            {
                mx.Error("检查更新时发生错误! ", ex);
            }

            return;
        }

        await ShowResultAsync(info, popup, isPreview, owner).ConfigureAwait(false);
    }

    public void InteractiveDownload(string version, string source)
    {
        if (string.IsNullOrEmpty(version) || !Version.TryParse(version, out _))
        {
            version = AppInfo.Version;
        }

        var src = default(UpdateSource);

        if (!string.IsNullOrEmpty(source))
        {
            if (int.TryParse(source, out var val) && Enum.IsDefined(typeof(UpdateSource), val))
            {
                src = (UpdateSource)val;
            }
            else
            {
                Enum.TryParse(source, true, out src);
            }
        }

        ShowDownloaderUI(string.Format(AppUpdate.GetDownloadUrl(src), version), 850 * 1024L);
    }

    private bool TryGetCachedResult(UpdateSource source, out AppUpdateInfo info)
    {
        lock (lockobj)
        {
            if (sources.TryGetValue(source, out var state)
                && state.CheckedAt != default
                && DateTime.TickCount - state.CheckedAt < ResultCacheMs)
            {
                info = state.Info;
                return true;
            }
        }

        info = null;
        return false;
    }

    private Session StartOrJoin(UpdateSource source, bool popup)
    {
        lock (lockobj)
        {
            if (!sources.TryGetValue(source, out var state))
            {
                state = new State();
                sources[source] = state;
            }

            if (state.Session == null || state.Session.Task.IsCompleted)
            {
                state.Session = new() { Task = FetchAsync(source, state) };
            }

            if (popup)
            {
                state.Session.PopupRequested = true;
            }

            return state.Session;
        }
    }

    private async Task<AppUpdateInfo> FetchAsync(UpdateSource source, State state)
    {
        using var cts = new CancellationTokenSource(RequestTimeoutMs);
        AppUpdateInfo info;

        try
        {
            info = await AppUpdate.FetchAsync(source, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            throw new TimeoutException($"请求超时，请稍后重试。", ex);
        }

        lock (lockobj)
        {
            state.Info = info;
            state.CheckedAt = DateTime.TickCount;
        }

        return info;
    }

    private async Task ShowResultAsync(AppUpdateInfo info, bool popup, bool isPreview, IAppWindow owner)
    {
        await dialogSemaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            ShowResult(info, popup, isPreview, owner);
        }
        finally
        {
            dialogSemaphore.Release();
        }
    }

    private void ShowResult(AppUpdateInfo info, bool popup, bool isPreview, IAppWindow owner)
    {
        var mx = owner.MessageX;
        var latest = info.Version;
        var date = info.ReleaseDate;
        var dateDesc = $"发布于 {date.FormatToTimeAgo()} ({date.Format()})";
        var content = info.Changelog;
        var sha = info.Commit;

        if (latest > App.Current.VersionObject
            || (isPreview && sha != AppInfo.CommitSHA && date.ToTimestamp() > AppInfo.Timestamp))
        {
            if (mx.Info(GetMessage(true, isPreview, latest, sha, dateDesc, content), MessageButtons.YesNo) == true)
            {
                owner.BeginInvoke((AppUpdateInfo i) => ShowDownloaderUI(i.Url, i.Size), info);
            }
        }
        else if (popup)
        {
            mx.Info(GetMessage(false, isPreview, latest, sha, dateDesc, content));
        }
    }

    private static string GetMessage(bool hasUpdate, bool isPreview, Version latest, string sha, string date, string content)
    {
        var sb = new StringBuilder(120);

        if (hasUpdate)
        {
            sb.AppendLine("检测到新版本，是否下载并安装？")
            .AppendLine()
            .AppendLine($"当前版本: v{AppInfo.Version}-{AppInfo.CommitSHA}")
            .Append("最新版本: v");
        }
        else
        {
            sb.AppendLine($"当前 v{AppInfo.Version}-{AppInfo.CommitSHA} 已是最新版本")
            .AppendLine()
            .Append("获取到的版本: v");
        }

        sb.Append(latest);

        if (!string.IsNullOrEmpty(sha))
        {
            sb.Append('-').Append(sha);
        }

        sb.AppendLine()
        .AppendLine(date);

        if (!isPreview)
        {
            sb.AppendLine();

            if (hasUpdate)
            {
                sb.Append('v')
                .Append(latest)
                .Append("更新日志: ");
            }
            else
            {
                sb.Append("当前版本更新日志: ");
            }

            sb.Append(content);
        }

        return sb.ToString();
    }

    private void ShowDownloaderUI(string url, long size)
    {
        if (FormDownloader == null || FormDownloader.IsDisposed)
        {
            FormDownloader = new(url, size);
        }

        if (Application.MessageLoop)
        {
            FormDownloader.ReActivate();
        }
        else
        {
            Application.Run(FormDownloader);
        }
    }
}
