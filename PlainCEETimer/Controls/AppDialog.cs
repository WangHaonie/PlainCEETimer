using PlainCEETimer.Modules;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class AppDialog : AppForm
    {
        protected Panel PanelMain { get; }
        protected PlainButton ButtonB { get; }
        protected PlainButton ButtonA { get; }

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
                ShowUnsavedWarning("是否保存当前更改？", e, ButtonA_Click, ref IsUserChanged);
            }
        }

        protected virtual bool ButtonA_Click()
        {
            IsUserChanged = false;
            DialogResult = DialogResult.OK;
            Close();
            return true;
        }

        protected virtual void ButtonB_Click()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected void UserChanged()
        {
            WhenLoaded(() =>
            {
                IsUserChanged = true;
                ButtonA.Enabled = true;
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
