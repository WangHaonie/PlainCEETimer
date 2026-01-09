using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Forms;

public sealed class AboutForm : AppForm
{
    protected override AppFormParam Params => AppFormParam.CenterScreen | AppFormParam.OnEscClosing;

    private bool IsCheckingUpdate;
    private PlainButton ButtonOK;
    private PlainLabel LabelAppName;
    private PlainLabel LabelLicense;
    private PlainLinkLabel Links;
    private PlainLinkLabel LinkVersion;
    private PictureBox ImageLogo;

    protected override void OnInitializing()
    {
        Text = "关于";
        const string buttonText = "确定(&O)";
        const string versionText = $"v{App.AppVersion} x64 ({AppInfo.BuildDate}, {AppInfo.CommitSHA})";

        this.AddControls(b =>
        [
            ImageLogo = b.Image(App.AppIcon.ToBitmap()).With(x =>
            {
                x.Cursor = Cursors.Help;

                x.MouseClick += (_, e) =>
                {
                    if (e.Button == MouseButtons.Left && !IsCheckingUpdate)
                    {
                        ImageLogo.Enabled = false;
                        ButtonOK.Text = "更新...";
                        ButtonOK.Enabled = false;
                        IsCheckingUpdate = true;

                        new Action(() => new Updater().CheckForUpdate(true, this)).Start(_ => Invoke(() =>
                        {
                            ImageLogo.Enabled = true;
                            ButtonOK.Text = buttonText;
                            ButtonOK.Enabled = true;
                            IsCheckingUpdate = false;
                        }));
                    }
                };
            }),

            LabelAppName = b.Label($"{App.AppNameEng}\n{App.AppName}"),

            LinkVersion = b.Hyperlink(versionText)
                .Link(versionText.Length - 8, 7, $"https://github.com/WangHaonie/PlainCEETimer/commit/{AppInfo.CommitSHA}", out _),

            LabelLicense = b.Label($"Licensed under the GNU GPL, v3.\n{App.CopyrightInfo}"),

            Links = b.Hyperlink("GitHub  反馈  教程")
                .Link(0, 6, "https://github.com/WangHaonie/PlainCEETimer", out _)
                .Link(8, 2, "https://github.com/WangHaonie/PlainCEETimer/issues/new/choose", out _)
                .Link(12, 2, "https://github.com/WangHaonie/PlainCEETimer/blob/main/.github/Manual.md", out _),

            ButtonOK = b.Button(buttonText, (_, _) => Close())
        ]);
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeControlXT(LabelAppName, ImageLogo, 0, isHighDpi ? -3 : 0);
        ArrangeControlYL(LinkVersion, ImageLogo, 1, -2);
        ArrangeControlYL(LabelLicense, ImageLogo, isHighDpi ? -3 : -2);
        CompactControlY(LabelLicense, LinkVersion, -4);
        ArrangeCommonButtonsR(null, ButtonOK, LabelLicense, -3);
        AlignControlXL(Links, LabelLicense, 2);
        CenterControlY(Links, ButtonOK, 2);
        LabelLicense.BringToFront();
    }

    protected override bool OnClosing(CloseReason closeReason)
    {
        return IsCheckingUpdate;
    }
}
