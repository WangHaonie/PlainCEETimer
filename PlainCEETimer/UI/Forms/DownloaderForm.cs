using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Http;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Forms
{
    public sealed class DownloaderForm : AppForm
    {
        private bool IsCancelled;
        private string DownloadUrl;
        private string DownloadPath;
        private Label LabelDownloading;
        private Label LabelSize;
        private Label LabelSpeed;
        private ProgressBar ProgressBarMain;
        private PlainButton ButtonRetry;
        private PlainButton ButtonCancel;
        private Hyperlink LinkBrowser;
        private readonly string TargetVersion;
        private readonly long UpdateSize;
        private readonly CancellationTokenSource cts = new();
        private readonly Downloader UpdateDownloader = new();

        private DownloaderForm() : base(AppFormParam.CenterScreen) { }

        public DownloaderForm(string ManualVersion) : this()
        {
            TargetVersion = Version.TryParse(ManualVersion, out _) ? ManualVersion : App.AppVersion;
        }

        public DownloaderForm(string Version, long Size) : this()
        {
            TargetVersion = Version;
            UpdateSize = Size;
        }

        protected override void OnInitializing()
        {
            Text = "更新进度 - 高考倒计时";

            this.AddControls(b =>
            [
                LabelDownloading = b.Label("正在下载更新文件，请稍侯..."),
                LinkBrowser = b.Hyperlink("浏览器下载", null),
                ProgressBarMain = b.New<ProgressBar>(344, 22, null),
                LabelSize = b.Label("已下载/总共: (获取中...)"),
                LabelSpeed = b.Label("下载速度: (获取中...)"),

                ButtonRetry = b.Button("重试(&R)", false, (_, _) =>
                {
                    ButtonRetry.Enabled = false;
                    ProgressBarMain.Value = 0;
                    UpdateLabels("正在重新下载更新文件，请稍侯...", "已下载/总共: (获取中...)", "下载速度: (获取中...)");
                    DownloadUpdate();
                }),

                ButtonCancel = b.Button("取消(&C)", (_, _) =>
                {
                    if (!IsCancelled)
                    {
                        ButtonCancel.Enabled = false;
                        cts.Cancel();
                        UpdateLabels("用户已取消下载。", null, null);
                        IsCancelled = true;
                        TaskbarProgress.SetState(TaskbarProgressState.Error);
                        MessageX.Warn("你已取消下载！\n\n稍后可以在 关于 窗口点击图标来再次检查更新。");
                    }

                    Close();
                })
            ]);
        }

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeFirstControl(LabelDownloading);
            ArrangeControlYL(ProgressBarMain, LabelDownloading, 2);
            ArrangeControlYL(LabelSize, ProgressBarMain, -2);
            ArrangeControlYL(LabelSpeed, LabelSize);
            ArrangeControlXT(LinkBrowser, LabelDownloading, 1);
            AlignControlYR(LinkBrowser, ProgressBarMain, 3);
            ArrangeCommonButtonsR(ButtonRetry, ButtonCancel, ProgressBarMain, 1, 6);
        }

        protected override void OnShown()
        {
            if (Win32User.NotElevated)
            {
                LinkBrowser.HyperLink = DownloadUrl = string.Format("https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/download/CEETimerCSharpWinForms_{0}_x64_Setup.exe", TargetVersion);
                TaskbarProgress.Initialize(Handle, (App.OSBuild >= WindowsBuilds.Windows7).ToWin32());
                DownloadPath = Path.Combine(Path.GetTempPath(), "PlainCEETimer-Installer.exe");
                UpdateDownloader.Downloading += UpdateDownloader_Downloading;
                UpdateDownloader.Error += UpdateDownloader_Error;
                UpdateDownloader.Completed += UpdateDownloader_Completed;
                DownloadUpdate();
            }
            else
            {
                MessageX.Error("系统环境异常，无法进行更新！");
                IsCancelled = true;
                Close();
            }
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return !IsCancelled;
        }

        protected override void OnClosed()
        {
            if (IsCancelled)
            {
                TaskbarProgress.SetState(TaskbarProgressState.None);
                TaskbarProgress.Release();
            }
        }

        private async void DownloadUpdate()
        {
            await UpdateDownloader.DownloadAsync(DownloadUrl, DownloadPath, cts.Token, UpdateSize);
        }

        private void UpdateDownloader_Downloading(DownloadReport report)
        {
            UpdateLabels(null, $"已下载/总共: {report.Downloaded} KB / {report.Total} KB", $"下载速度: {report.Speed:0.00} KB/s");
            ProgressBarMain.Value = report.Progress;
            TaskbarProgress.SetValue((ulong)report.Downloaded, (ulong)report.Total);
        }

        private void UpdateDownloader_Error(Exception ex)
        {
            MessageX.Error("无法下载更新文件！", ex);
            UpdateLabels("下载失败，你可以点击 重试 来重新启动下载。", "已下载/总共: N/A", "下载速度: N/A");
            ButtonRetry.Enabled = true;
            TaskbarProgress.SetValue(1UL, 1UL);
            TaskbarProgress.SetState(TaskbarProgressState.Error);
        }

        private void UpdateDownloader_Completed()
        {
            ButtonCancel.Enabled = false;
            ButtonRetry.Enabled = false;
            LinkBrowser.Enabled = false;
            ProgressBarMain.Value = 100;
            TaskbarProgress.SetValue(1UL, 1UL);
            TaskbarProgress.SetState(TaskbarProgressState.Indeterminate);
            UpdateLabels("下载完成，请稍侯...", null, null);
            IsCancelled = true;

            2500.AsDelay(_ => Invoke(() =>
            {
                ProcessHelper.Run(DownloadPath, "/Skip");
                App.Exit(ExitReason.AppUpdating);
            }));
        }

        private void UpdateLabels(string Info, string Size, string Speed)
        {
            if (Info != null)
            {
                LabelDownloading.Text = Info;
            }

            if (Size != null)
            {
                LabelSize.Text = Size;
            }

            if (Speed != null)
            {
                LabelSpeed.Text = Speed;
            }
        }
    }
}