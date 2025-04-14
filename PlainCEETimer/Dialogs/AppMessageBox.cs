using PlainCEETimer.Controls;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules;
using System.Drawing;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public partial class AppMessageBox : AppDialog
    {
        private DialogResult Result;
        private readonly bool AutoCloseRequired;
        private readonly MessageButtons ButtonsEx;
        private readonly SystemSound DialogSound;

        public AppMessageBox(SystemSound Sound, MessageButtons Buttons, bool AutoClose) : base(AppDialogProp.KeyPreview)
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
            StartPosition = (OwnerForm != null && MainForm.IsNormalStart) ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;
            ShowDialog(OwnerForm);
            return Result;
        }

        protected override void AdjustUI()
        {
            PanelMain.AutoSize = true;
            PanelMain.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SetLabelAutoWrap(LabelMessage);
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
                Task.Run(() => Task.Delay(3200)).ContinueWith(t => BeginInvoke(Close));
            }
        }

        protected override bool ButtonA_Click()
        {
            Result = ButtonsEx == MessageButtons.YesNo ? DialogResult.Yes : DialogResult.None;
            Close();
            return true;
        }

        protected override void ButtonB_Click()
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
