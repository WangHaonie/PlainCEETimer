using Newtonsoft.Json;
using PlainCEETimer.Controls;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.Extensions;
using System;
using System.Net.Http;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public sealed class Updater
    {
        private DownloaderForm FormDownloader;

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

                    if (MessageX.Info($"检测到新版本，是否下载并安装？\n\n当前版本: v{App.AppVersion}\n最新版本: v{LatestVersion}\n发布日期: {PublishDate}\n\nv{LatestVersion}更新日志: {UpdateLog}", Buttons: MessageBoxExButtons.YesNo) == DialogResult.Yes)
                    {
                        OwnerForm.BeginInvoke(() =>
                        {
                            if (FormDownloader == null || FormDownloader.IsDisposed)
                            {
                                FormDownloader = new(LatestVersion, Response.UpdateSize);
                            }

                            FormDownloader.ReActivate();
                        });
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
                    MessageX.Error($"检查更新时发生错误! {ex.ToMessage()}");
                }
            }
        }
    }

    public sealed class ResponseObject
    {
        [JsonProperty("name")]
        public string Version { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishDate { get; set; }

        [JsonProperty("size")]
        public long UpdateSize { get; set; }

        [JsonProperty("body")]
        public string UpdateLog { get; set; }
    }
}
