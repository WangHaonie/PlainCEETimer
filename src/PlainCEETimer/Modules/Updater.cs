using System;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Http;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules;

public class Updater
{
    private DownloaderForm FormDownloader;

    public void CheckForUpdate(bool popup, AppForm owner, bool isPreview = false)
    {
        var MessageX = owner.MessageX;

        try
        {
            var url = isPreview
                ? "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/api/ci.json"
                : "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/api/github.json";

            var response = JsonConvert.DeserializeObject<ResponseObject>(HttpService.GetStringAsync(url).Result);
            var latest = response.Version;
            var date = response.PublishDate;
            var datep = date.ToTimestamp();
            var dateDesc = $"发布于 {GetDescription(date)} ({date.Format()})";
            var content = response.UpdateLog.Replace("\n\r", "\n\r· ");
            var sha = response.Commit;

            if (Version.Parse(latest) > App.VersionObject || isPreview && sha != AppInfo.CommitSHA && datep > AppInfo.Timestamp)
            {
                if (MessageX.Info(GetMessage(true, isPreview, latest, sha, dateDesc, content), MessageButtons.YesNo) == DialogResult.Yes)
                {
                    owner.BeginInvoke(() =>
                    {
                        if (FormDownloader == null || FormDownloader.IsDisposed)
                        {
                            FormDownloader = new(isPreview, latest, response.UpdateSize);
                        }

                        FormDownloader.ReActivate();
                    });
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

    private string GetMessage(bool hasUpdate, bool isPreview, string latest, string sha, string date, string content)
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
}
