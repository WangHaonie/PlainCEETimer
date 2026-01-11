using System;
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
            var response = JsonConvert.DeserializeObject<ResponseObject>(HttpService.GetStringAsync("https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/api/github.json").Result);
            var latest = response.Version;
            var date = response.PublishDate;
            var dateDesc = $"发布于 {GetDescription(date)} ({date.Format()})";
            var content = response.UpdateLog.Replace("\n", "\n· ");

            if (Version.Parse(latest) > App.VersionObject)
            {
                if (MessageX.Info(
                    $"""
                    检测到新版本，是否下载并安装？
                    
                    当前版本: v{AppInfo.Version}
                    最新版本: v{latest}
                    {dateDesc}
                    
                    v{latest}更新日志: {content}
                    """, MessageButtons.YesNo) == DialogResult.Yes)
                {
                    owner.BeginInvoke(() =>
                    {
                        if (FormDownloader == null || FormDownloader.IsDisposed)
                        {
                            FormDownloader = new(latest, response.UpdateSize);
                        }

                        FormDownloader.ReActivate();
                    });
                }
            }
            else if (popup)
            {
                MessageX.Info(
                    $"""
                    当前 v{AppInfo.Version} 已是最新版本。
                    
                    获取到的版本: v{latest}
                    {dateDesc}
                    
                    当前版本更新日志: {content}
                    """);
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
}
