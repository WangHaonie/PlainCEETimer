using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Forms
{
    public partial class AboutForm : AppForm
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
                CompactControlsY(ButtonClose, LabelLicense);
                AlignControlsX([LinkGitHub, LinkFeedback, LinkTutorial], ButtonClose);
                CompactControlsX(LinkFeedback, LinkGitHub);
                CompactControlsX(LinkTutorial, LinkFeedback);
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
                Task.Run(() => new Updater().CheckForUpdate(false, this)).ContinueWith(t => BeginInvoke(() =>
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
