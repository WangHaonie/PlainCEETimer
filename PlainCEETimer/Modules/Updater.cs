using Newtonsoft.Json;
using PlainCEETimer.Controls;
using PlainCEETimer.Forms;
using System;
using System.Net.Http;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public sealed class Updater
    {
        public void CheckForUpdate(bool IsProgramStart, AppForm OwnerForm)
        {
            var MessageX = new MessageBoxHelper(OwnerForm);
            using var _HttpClient = new HttpClient();
            _HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(App.RequestUA);

            try
            {
                var Response = JsonConvert.DeserializeObject<ResponseObject>(_HttpClient.GetAsync(App.UpdateAPI).Result.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result);

                var LatestVersion = Response.Version;
                var PublishDate = Response.PublishDate;
                var UpdateLog = Response.UpdateLog;

                if (Version.Parse(LatestVersion) > Version.Parse(App.AppVersion))
                {
                    if (MessageX.Info($"检测到新版本，是否下载并安装？\n\n当前版本: v{App.AppVersion}\n最新版本: v{LatestVersion}\n发布日期: {PublishDate}\n\nv{LatestVersion}更新日志: {UpdateLog}", Buttons: MessageButtons.YesNo) == DialogResult.Yes)
                    {
                        OwnerForm.BeginInvoke(() => Downloader.Start(LatestVersion, Response.UpdateSize));
                    }
                }
                else if (!IsProgramStart)
                {
                    MessageX.Info($"当前 v{App.AppVersion} 已是最新版本。\n\n获取到的版本: v{LatestVersion}\n发布日期: {PublishDate}\n\n当前版本更新日志: {UpdateLog}");
                }
            }
            catch (Exception ex)
            {
                if (!IsProgramStart)
                {
                    MessageX.Error("检查更新时发生错误! ", ex);
                }
            }
        }

        private static class Downloader
        {
            private static DownloaderForm FormDownloader;

            public static void Start(string version, long size)
            {
                if (FormDownloader == null || FormDownloader.IsDisposed)
                {
                    FormDownloader = new(version, size);
                }

                FormDownloader.ReActivate();
            }
        }
    }
}
