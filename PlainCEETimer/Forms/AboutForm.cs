using System;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Forms
{
    public sealed partial class AboutForm : AppForm
    {
        private bool IsCheckingUpdate;

        public AboutForm() : base(AppFormParam.CenterScreen)
        {
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            PicBoxLogo.Image = App.AppIcon.ToBitmap();
            LabelInfo.Text = $"{App.AppName}\n版本 v{App.AppVersion} x64 ({App.AppBuildDate})";
            LabelLicense.Text = $"Licensed under the GNU GPL, v3.\n{App.CopyrightInfo}";

            WhenHighDpi(() =>
            {
                CompactControlsY(ButtonOK, LabelLicense);
                CompactControlsX(LinkFeedback, LinkGitHub);
                CompactControlsX(LinkTutorial, LinkFeedback);
                AlignControlsX([LinkGitHub, LinkFeedback, LinkTutorial], ButtonOK);
                AlignControlsR(ButtonOK, LabelLicense);
            });
        }

        protected override void OnClosing(FormClosingEventArgs e)
        {
            e.Cancel = IsCheckingUpdate;
        }

        private void PicBoxLogo_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !IsCheckingUpdate)
            {
                var OriginalVersionString = LabelInfo.Text;
                IsCheckingUpdate = true;
                PicBoxLogo.Enabled = false;
                LabelInfo.Text = $"{App.AppName}\n正在检查更新，请稍候...";
                new Action(() => new Updater().CheckForUpdate(true, this)).Start(_ => Invoke(() =>
                {
                    LabelInfo.Text = OriginalVersionString;
                    PicBoxLogo.Enabled = true;
                    IsCheckingUpdate = false;
                }));
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
