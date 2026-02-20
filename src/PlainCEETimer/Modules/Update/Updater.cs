using System;
using System.Text;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules.Update;

internal class Updater
{
    private DownloaderForm FormDownloader;

    public void CheckForUpdate(bool popup, AppForm owner, bool isPreview = false)
    {
        var MessageX = owner.MessageX;

        try
        {
            var response = AppUpdate.FetchAsync(UpdateSource.GiteeStable).Result;
            var latest = response.Version;
            var date = response.ReleaseDate;
            var dateDesc = $"发布于 {GetDescription(date)} ({date.Format()})";
            var content = response.Changelog;
            var sha = response.Commit;

            if (latest > App.VersionObject
                || (isPreview && sha != AppInfo.CommitSHA && date.ToTimestamp() > AppInfo.Timestamp))
            {
                if (MessageX.Info(GetMessage(true, isPreview, latest, sha, dateDesc, content), MessageButtons.YesNo) == DialogResult.Yes)
                {
                    owner.BeginInvoke(() => ShowDownloaderUI(response.Url, response.Size));
                }
            }
            else if (popup)
            {
                MessageX.Info(GetMessage(false, isPreview, latest, sha, dateDesc, content));
            }
        }
        catch (Exception ex)
        {
            if (popup)
            {
                MessageX.Error("检查更新时发生错误! ", ex);
            }
        }
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

        ShowDownloaderUI(string.Format(AppUpdate.GetDownloadUrl(src), version), 370 * 1024L);
    }

    private string GetDescription(DateTime pub)
    {
        var span = DateTime.Now - pub;
        var tm = (int)span.TotalMinutes;
        var th = span.TotalHours;

        if ((int)span.TotalSeconds < 60) return "刚刚";
        if (tm < 60) return $"{tm} 分钟前";
        if (th < 24D) return $"{th:0.0} 小时前";
        return $"{span.TotalDays:0.0} 天前";
    }

    private string GetMessage(bool hasUpdate, bool isPreview, Version latest, string sha, string date, string content)
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
