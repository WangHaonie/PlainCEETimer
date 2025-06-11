using System;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.WinForms;

namespace PlainCEETimer.Forms
{
    public sealed class AboutForm : AppForm
    {
        private bool IsCheckingUpdate;
        private string VersionString;
        private PlainButton ButtonOK;
        private Label LabelInfo;
        private Label LabelLicense;
        private Hyperlink LinkGitHub;
        private PictureBox ImageLogo;
        private Hyperlink LinkFeedback;
        private Hyperlink LinkTutorial;

        public AboutForm() : base(AppFormParam.CenterScreen) { }

        protected override void OnInitializing()
        {
            VersionString = $"{App.AppName}\n版本 v{App.AppVersion} x64 ({App.AppBuildDate})";
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
                            LabelInfo.Text = $"{App.AppName}\n正在检查更新，请稍候...";

                            new Action(() => new Updater().CheckForUpdate(true, this)).Start(_ => Invoke(() =>
                            {
                                LabelInfo.Text = VersionString;
                                ImageLogo.Enabled = true;
                                IsCheckingUpdate = false;
                            }));
                        }
                    };
                }),

                LabelInfo = b.Label(VersionString),
                LabelLicense = b.Label($"Licensed under the GNU GPL, v3.\n{App.CopyrightInfo}"),
                LinkGitHub = b.Hyperlink("GitHub", "https://github.com/WangHaonie/PlainCEETimer"),
                LinkFeedback = b.Hyperlink("反馈", "https://github.com/WangHaonie/PlainCEETimer/issues/new/choose"),
                LinkTutorial = b.Hyperlink("教程", "https://github.com/WangHaonie/PlainCEETimer/blob/main/.github/Manual.md"),
                ButtonOK = b.Button("确定(&O)", (_, _) => Close())
            ]);
        }

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeControlXTop(LabelInfo, ImageLogo, 0, isHighDpi ? -3 : 0);
            ArrangeControlYLeft(LabelLicense, ImageLogo, -2);
            CompactControlY(LabelLicense, LabelInfo);
            ArrangeControlYRight(ButtonOK, LabelLicense, -3);
            AlignControlLeft(LinkGitHub, LabelLicense);
            CenterControlY(LinkGitHub, ButtonOK);
            ArrangeControlXTop(LinkFeedback, LinkGitHub);
            ArrangeControlXTop(LinkTutorial, LinkFeedback);
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsCheckingUpdate;
        }
    }
}
