using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Forms
{
    public sealed class AboutForm : AppForm
    {
        private bool IsCheckingUpdate;
        private PlainButton ButtonOK;
        private PlainLabel LabelInfo;
        private PlainLabel LabelLicense;
        private Hyperlink LinkGitHub;
        private PictureBox PicBoxLogo;
        private Hyperlink LinkFeedback;
        private Hyperlink LinkTutorial;
        private readonly string VersionString;

        public AboutForm() : base(AppFormParam.CenterScreen)
        {
            SuspendLayout();
            AcceptButton = ButtonOK;
            AutoScaleDimensions = new(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new(208, 94);
            Font = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ShowIcon = false;
            Text = "关于";
            VersionString = $"{App.AppName}\n版本 v{App.AppVersion} x64 ({App.AppBuildDate})";

            this.AddControls(b =>
            [
                PicBoxLogo = b.New<PictureBox>(3, 3, 32, 32, null, c =>
                {
                    c.Cursor = Cursors.Help;
                    c.Image = App.AppIcon.ToBitmap();
                    c.SizeMode = PictureBoxSizeMode.Zoom;

                    c.MouseClick += (_, e) =>
                    {
                        if (e.Button == MouseButtons.Left && !IsCheckingUpdate)
                        {
                            IsCheckingUpdate = true;
                            PicBoxLogo.Enabled = false;
                            LabelInfo.Text = $"{App.AppName}\n正在检查更新，请稍候...";

                            new Action(() => new Updater().CheckForUpdate(true, this)).Start(_ => Invoke(() =>
                            {
                                LabelInfo.Text = VersionString;
                                PicBoxLogo.Enabled = true;
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
                ButtonOK = b.Button(0, 0, "确定(&O)")
            ]);
            ResumeLayout(true);
        }

        private void AlignControlT(Control Target, Control Reference, int Tweak = 0)
        {
            Target.Top = Reference.Top + ScaleToDpi(Tweak);
        }

        protected override void OnLoad()
        {
            AlignControlT(LabelInfo, PicBoxLogo);
            AlignControlsL(LabelLicense, PicBoxLogo, -2);
            AlignControlsL(LinkGitHub, LabelLicense);
            CompactControlsX(LabelInfo, PicBoxLogo);
            CompactControlsY(LabelLicense, LabelInfo);
            CompactControlsY(ButtonOK, LabelLicense);
            PicBoxLogo.Top = LabelLicense.Top - PicBoxLogo.Height;
            CompactControlsX(LinkFeedback, LinkGitHub);
            CompactControlsX(LinkTutorial, LinkFeedback);
            AlignControlsX([LinkGitHub, LinkFeedback, LinkTutorial], ButtonOK);
            AlignControlsR(ButtonOK, LabelLicense);
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsCheckingUpdate;
        }
    }
}
