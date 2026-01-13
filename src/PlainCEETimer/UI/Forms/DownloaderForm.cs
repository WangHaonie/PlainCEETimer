using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Http;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PlainCEETimer.UI.Forms;

public sealed class DownloaderForm : AppForm
{
    protected override AppFormParam Params => AppFormParam.CenterScreen | AppFormParam.OnEscClosing;

    private bool IsCancelled;
    private bool IsPreview;
    private string DownloadUrl;
    private string DownloadPath;
    private PlainLabel LabelDownloading;
    private PlainLabel LabelSize;
    private PlainLabel LabelSpeed;
    private PlainProgressBar ProgressBarMain;
    private PlainButton ButtonRetry;
    private PlainButton ButtonCancel;
    private PlainLinkLabel LinkBrowser;
    private LinkLabel.Link LinkBrowserLink;
    private readonly string TargetVersion;
    private readonly long UpdateSize;
    private readonly CancellationTokenSource cts = new();
    private readonly Downloader UpdateDownloader = new();

    public DownloaderForm(string version)
    {
        IsPreview = version == "/pre";
        TargetVersion = Version.TryParse(version, out _) ? version : AppInfo.Version;
    }

    public DownloaderForm(bool isPreview, string version, long size)
    {
        IsPreview = isPreview;
        TargetVersion = version;
        UpdateSize = size;
    }

    protected override void OnInitializing()
    {
        Text = "更新进度 - 高考倒计时";

        this.AddControls(b =>
        [
            LabelDownloading = b.Label("正在下载更新文件，请稍侯..."),
            LinkBrowser = b.Hyperlink("浏览器下载").Link(null, out LinkBrowserLink),
            ProgressBarMain = b.New<PlainProgressBar>(344, 22, null),
            LabelSize = b.Label("已下载/总共: (获取中...)"),
            LabelSpeed = b.Label("下载速度: (获取中...)"),

            ButtonRetry = b.Button("重试(&R)", (_, _) =>
            {
                ProgressBarMain.Style = ProgressStyle.Normal;
                ProgressBarMain.Value = 0;
                UpdateLabels("正在重新下载更新文件，请稍侯...", "已下载/总共: (获取中...)", "下载速度: (获取中...)");
                DownloadUpdate();
            }).Disable(),

            ButtonCancel = b.Button("取消(&C)", (_, _) =>
            {
                if (!IsCancelled)
                {
                    ButtonCancel.Enabled = false;
                    cts.Cancel();
                    UpdateLabels("用户已取消下载。", null, null);
                    IsCancelled = true;
                    ProgressBarMain.Style = ProgressStyle.Paused;
                    MessageX.Warn("你已取消下载！\n\n稍后可以在 关于 窗口点击图标来再次检查更新。");
                }

                Close();
            })
        ]);
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeFirstControl(LabelDownloading);
        ArrangeControlYL(ProgressBarMain, LabelDownloading, 2);
        ArrangeControlYL(LabelSize, ProgressBarMain, -2);
        ArrangeControlYL(LabelSpeed, LabelSize);
        ArrangeControlXT(LinkBrowser, LabelDownloading, 1);
        AlignControlXR(LinkBrowser, ProgressBarMain, 3);
        ArrangeCommonButtonsR(ButtonRetry, ButtonCancel, ProgressBarMain, 1, 6);
    }

    protected override void OnShown()
    {
        if (Win32User.NotImpersonalOrElevated)
        {
            var sb = new StringBuilder(120)
            .Append("https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/download/");

            if (IsPreview)
            {
                sb.Append("ci/");
            }

            sb.Append("CEETimerCSharpWinForms_")
            .Append(TargetVersion)
            .Append("_x64_Setup.exe");

            LinkBrowserLink.LinkData = DownloadUrl = sb.ToString();
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

    private async void DownloadUpdate()
    {
        IsCancelled = false;
        ButtonRetry.Enabled = false;
        await UpdateDownloader.DownloadAsync(DownloadUrl, DownloadPath, cts.Token, UpdateSize);
    }

    private void UpdateDownloader_Downloading(object sender, ref DownloadReport report)
    {
        UpdateLabels(null, $"已下载/总共: {report.Downloaded} KB / {report.Total} KB", $"下载速度: {report.Speed:0.00} KB/s");
        ProgressBarMain.Value = report.Progress;
    }

    private void UpdateDownloader_Error(Exception ex)
    {
        MessageX.Error("无法下载更新文件！", ex);
        UpdateLabels("下载失败，你可以点击 重试 来重新启动下载。", "已下载/总共: N/A", "下载速度: N/A");
        ButtonRetry.Enabled = true;
        IsCancelled = true;
        ProgressBarMain.Value = 100;
        ProgressBarMain.Style = ProgressStyle.Error;
    }

    private void UpdateDownloader_Completed()
    {
        ButtonCancel.Enabled = false;
        ButtonRetry.Enabled = false;
        LinkBrowserLink.Enabled = false;
        ProgressBarMain.Value = 100;
        ProgressBarMain.Style = ProgressStyle.Indeterminate;
        UpdateLabels("下载完成，请稍侯...", null, null);
        IsCancelled = true;

        2500.AsDelay(() =>
        {
            ProcessHelper.Run(DownloadPath, "/Skip");
            App.Exit();
        }, this);
    }

    private void UpdateLabels(string info, string size, string speed)
    {
        if (info != null)
        {
            LabelDownloading.Text = info;
        }

        if (size != null)
        {
            LabelSize.Text = size;
        }

        if (speed != null)
        {
            LabelSpeed.Text = speed;
        }
    }
}
