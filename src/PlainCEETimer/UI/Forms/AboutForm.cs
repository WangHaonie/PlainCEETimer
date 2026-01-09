using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Properties;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Forms;

public sealed class AboutForm : AppForm
{
    protected override AppFormParam Params => AppFormParam.CenterScreen | AppFormParam.OnEscClosing;

    private bool IsCheckingUpdate;
    private PlainButton ButtonOK;
    private PlainLabel LabelAppName;
    private PlainLabel LabelVersion;
    private PlainLabel LabelLicense;
    private PlainLinkLabel LinkCommit;
    private PlainLinkLabel LinkGitHub;
    private PlainLinkLabel LinkFeedback;
    private PlainLinkLabel LinkTutorial;
    private PictureBox ImageLogo;

    protected override void OnInitializing()
    {
        Text = "关于";

        this.AddControls(b =>
        [
            ImageLogo = b.Image(App.AppIcon.ToBitmap()).With(x =>
            {
                x.Cursor = Cursors.Help;

                x.MouseClick += (_, e) =>
                {
                    if (e.Button == MouseButtons.Left && !IsCheckingUpdate)
                    {
                        IsCheckingUpdate = true;
                        ImageLogo.Enabled = false;
                        LabelAppName.Text = $"{App.AppName}\n正在检查更新，请稍候...";

                        new Action(() => new Updater().CheckForUpdate(true, this)).Start(_ => Invoke(() =>
                        {
                            LabelAppName.Text = App.AppName;
                            ImageLogo.Enabled = true;
                            IsCheckingUpdate = false;
                        }));
                    }
                };
            }),

            LabelAppName = b.Label(App.AppName),
            LabelVersion = b.Label($"v{App.AppVersion} x64 {AppInfo.BuildDate}"),
            LinkCommit = b.Hyperlink(AppInfo.CommitSHA, $"https://github.com/WangHaonie/PlainCEETimer/commit/{AppInfo.CommitSHA}"),
            LabelLicense = b.Label($"Licensed under the GNU GPL, v3.\n{App.CopyrightInfo}"),
            LinkGitHub = b.Hyperlink("GitHub", "https://github.com/WangHaonie/PlainCEETimer"),
            LinkFeedback = b.Hyperlink("反馈", "https://github.com/WangHaonie/PlainCEETimer/issues/new/choose"),
            LinkTutorial = b.Hyperlink("教程", "https://github.com/WangHaonie/PlainCEETimer/blob/main/.github/Manual.md"),
            ButtonOK = b.Button("确定(&O)", (_, _) => Close())
        ]);
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeControlXT(LabelAppName, ImageLogo, 0, isHighDpi ? -3 : 0);
        ArrangeControlYL(LabelVersion, LabelAppName);
        ArrangeControlXT(LinkCommit, LabelVersion);
        ArrangeControlYL(LabelLicense, ImageLogo, isHighDpi ? -3 : -2);
        CompactControlY(LabelLicense, LabelVersion);
        ArrangeCommonButtonsR(null, ButtonOK, LabelLicense, -3);
        AlignControlXL(LinkGitHub, LabelLicense);
        CenterControlY(LinkGitHub, ButtonOK);
        ArrangeControlXT(LinkFeedback, LinkGitHub);
        ArrangeControlXT(LinkTutorial, LinkFeedback);
    }

    protected override bool OnClosing(CloseReason closeReason)
    {
        return IsCheckingUpdate;
    }
}
