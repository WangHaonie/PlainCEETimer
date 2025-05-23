using System.Drawing;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Dialogs
{
    public sealed partial class AppMessageBox : AppDialog
    {
        private DialogResult Result;
        private readonly bool AutoCloseRequired;
        private readonly MessageButtons ButtonsEx;
        private readonly SystemSound DialogSound;

        public AppMessageBox(SystemSound Sound, MessageButtons Buttons, bool AutoClose) : base(AppFormParam.KeyPreview)
        {
            InitializeComponent();
            DialogSound = Sound;
            ButtonsEx = Buttons;
            AutoCloseRequired = AutoClose;
        }

        public DialogResult ShowCore(AppForm OwnerForm, string Message, string Title, Bitmap AppMessageBoxIcon)
        {
            LabelMessage.Text = Message;
            Text = Title;
            PicBoxIcon.Image = AppMessageBoxIcon;

            if (OwnerForm == null)
            {
                AddParam(AppFormParam.CenterScreen);
            }
            else
            {
                StartPosition = FormStartPosition.CenterParent;
            }

            ShowDialog(OwnerForm);
            return Result;
        }

        protected override void AdjustUI()
        {
            PanelMain.AutoSize = true;
            PanelMain.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SetLabelAutoWrap(LabelMessage, (int)(GetCurrentScreenRect().Width * 0.75));
            AlignControlsR(ButtonA, ButtonB, PanelMain);
        }

        protected override void OnLoad()
        {
            switch (ButtonsEx)
            {
                case MessageButtons.YesNo:
                    ButtonA.Text = "是(&Y)";
                    ButtonB.Text = "否(&N)";
                    break;
                case MessageButtons.OK:
                    ButtonA.Visible = ButtonA.Enabled = false;
                    ButtonB.Text = "确定(&O)";
                    break;
            }
        }

        protected override void OnShown()
        {
            DialogSound.Play();

            if (AutoCloseRequired)
            {
                Task.Run(() => Task.Delay(3200)).ContinueWith(_ => BeginInvoke(Close));
            }
        }

        protected override bool OnClickButtonA()
        {
            Result = ButtonsEx == MessageButtons.YesNo ? DialogResult.Yes : DialogResult.None;
            Close();
            return true;
        }

        protected override void OnClickButtonB()
        {
            Result = ButtonsEx == MessageButtons.YesNo ? DialogResult.No : DialogResult.OK;
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }

            base.OnKeyDown(e);
        }
    }
}
