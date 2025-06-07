using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public class AppDialog : AppForm
    {
        protected readonly Panel PanelMain;
        protected readonly PlainButton ButtonA;
        protected readonly PlainButton ButtonB;

        private bool IsUserChanged;

        protected AppDialog(AppFormParam param) : base(param)
        {
            PanelMain = new();
            ButtonA = new() { UseVisualStyleBackColor = true };
            ButtonB = new() { UseVisualStyleBackColor = true };
            ButtonA.Click += (_, _) => OnClickButtonA();
            ButtonB.Click += (_, _) => OnClickButtonB();
            SetProperties();
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsUserChanged && ShowUnsavedWarning("是否保存当前更改？", OnClickButtonA, ref IsUserChanged);
        }

        protected virtual bool OnClickButtonA()
        {
            IsUserChanged = false;
            DialogResult = DialogResult.OK;
            Close();
            return true;
        }

        protected virtual void OnClickButtonB()
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
