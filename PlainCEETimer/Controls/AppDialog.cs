using PlainCEETimer.Modules;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class AppDialog : AppForm
    {
        protected Panel PanelMain { get; }
        protected Button ButtonB { get; }
        protected Button ButtonA { get; }

        private bool IsUserChanged;

        private AppDialog()
        {
            PanelMain = new();
            ButtonA = new();
            ButtonB = new();
            ButtonA.Click += (_, _) => ButtonA_Click();
            ButtonB.Click += (_, _) => ButtonB_Click();
        }

        protected AppDialog(AppDialogProp Prop) : this()
        {
            SetProperties(Prop);
        }

        protected override void OnClosing(FormClosingEventArgs e)
        {
            if (App.AllowClosing)
            {
                e.Cancel = false;
            }
            else if (IsUserChanged)
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

        protected void UserChanged()
        {
            WhenLoaded(() =>
            {
                if (!ButtonA.Enabled)
                {
                    IsUserChanged = true;
                    ButtonA.Enabled = true;
                }
            });
        }

        private void SetProperties(AppDialogProp Prop)
        {
            if ((Prop & AppDialogProp.BindButtons) != 0)
            {
                AcceptButton = ButtonA;
                CancelButton = ButtonB;
            }

            if ((Prop & AppDialogProp.KeyPreview) != 0)
            {
                KeyPreview = true;
            }
        }
    }
}
