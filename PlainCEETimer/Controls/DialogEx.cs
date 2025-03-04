using PlainCEETimer.Modules;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class DialogEx : AppForm
    {
        protected Panel PanelMain { get; private set; }
        protected Button ButtonB { get; private set; }
        protected Button ButtonA { get; private set; }

        private bool IsUserChanged;

        private DialogEx()
        {
            InitializeComponent();
        }

        protected DialogEx(DialogExProp Prop) : this()
        {
            SetProperties(Prop);
        }

        private void InitializeComponent()
        {
            PanelMain = new();
            ButtonA = new();
            ButtonB = new();
            ButtonA.Click += (_, _) => ButtonA_Click();
            ButtonB.Click += (_, _) => ButtonB_Click();
        }

        protected override void OnClosing(FormClosingEventArgs e)
        {
            if (IsUserChanged)
            {
                ShowUnsavedWarning("是否保存当前更改？", e, ButtonA_Click, () =>
                {
                    IsUserChanged = false;
                    Close();
                });
            }
        }

        protected virtual void ButtonA_Click()
        {
            IsUserChanged = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected virtual void ButtonB_Click()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected void AdjustPanel()
        {
            AlignControlsR(ButtonA, ButtonB, PanelMain);
        }

        protected void EnablePanelAutoSize(AutoSizeMode Mode)
        {
            PanelMain.AutoSize = true;
            PanelMain.AutoSizeMode = Mode;
        }

        protected void UserChanged()
        {
            Execute(() =>
            {
                if (!ButtonA.Enabled)
                {
                    IsUserChanged = true;
                    ButtonA.Enabled = true;
                }
            });
        }

        private void SetProperties(DialogExProp Prop)
        {
            if ((Prop & DialogExProp.BindButtons) != 0)
            {
                AcceptButton = ButtonA;
                CancelButton = ButtonB;
            }

            if ((Prop & DialogExProp.KeyPreview) != 0)
            {
                KeyPreview = true;
            }
        }
    }
}
