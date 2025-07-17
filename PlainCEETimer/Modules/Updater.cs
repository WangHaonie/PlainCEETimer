using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Http;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules
{
    public class Updater
    {
        private DownloaderForm FormDownloader;

        public void CheckForUpdate(bool popup, AppForm owner)
        {
            var MessageX = owner.MessageX;

            try
            {
                var Response = JsonConvert.DeserializeObject<ResponseObject>(HttpService.GetStringAsync("https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/api/github.json").Result);
                var LatestVersion = Response.Version;
                var pub = Response.PublishDate;
                var PublishDate = $"发布于 {GetDescription(pub)} ({pub.Format()})";
                var UpdateLog = Response.UpdateLog.Replace("\n", "\n· ");

                if (Version.Parse(LatestVersion) > App.AppVersionObject)
                {
                    if (MessageX.Info($"检测到新版本，是否下载并安装？\n\n当前版本: v{App.AppVersion}\n最新版本: v{LatestVersion}\n{PublishDate}\n\nv{LatestVersion}更新日志: {UpdateLog}", MessageButtons.YesNo) == DialogResult.Yes)
                    {
                        owner.BeginInvoke(() =>
                        {
                            if (FormDownloader == null || FormDownloader.IsDisposed)
                            {
                                FormDownloader = new(LatestVersion, Response.UpdateSize);
                            }

                            FormDownloader.ReActivate();
                        });
                    }
                }
                else if (popup)
                {
                    MessageX.Info($"当前 v{App.AppVersion} 已是最新版本。\n\n获取到的版本: v{LatestVersion}\n{PublishDate}\n\n当前版本更新日志: {UpdateLog}");
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
}
