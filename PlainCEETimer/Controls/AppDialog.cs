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

        protected AppDialog(AppFormParam param) : base(param)
        {
            PanelMain = new();
            ButtonA = new();
            ButtonB = new();
            ButtonA.Click += (_, _) => ButtonA_Click();
            ButtonB.Click += (_, _) => ButtonB_Click();
            SetProperties();
        }

        protected override void OnClosing(FormClosingEventArgs e)
        {
            if (App.AllowUIClosing)
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

        private void SetProperties()
        {
            if (CheckParam(AppFormParam.BindButtons))
            {
                AcceptButton = ButtonA;
                CancelButton = ButtonB;
            }

            if (CheckParam(AppFormParam.KeyPreview))
            {
                KeyPreview = true;
            }
        }
    }
}
