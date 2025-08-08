using System.Windows.Forms;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls
{
    public abstract class AppDialog(AppFormParam param) : AppForm(param)
    {
        protected PlainButton ButtonA { get; private set; }
        protected PlainButton ButtonB { get; private set; }

        private bool IsUserChanged;

        protected override void OnInitializing()
        {
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;

            this.AddControls(b =>
            [
                ButtonA = b.Button("确定(&O)", false, (_, _) => OnClickButtonA()),
                ButtonB = b.Button("取消(&C)", (_, _) => OnClickButtonB())
            ]);

            if (CheckParam(AppFormParam.BindButtons))
            {
                AcceptButton = ButtonA;
                CancelButton = ButtonB;
            }
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

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsUserChanged && ShowUnsavedWarning("是否保存当前更改？", OnClickButtonA, ref IsUserChanged);
        }

        protected void UserChanged()
        {
            EnsureLoaded(() =>
            {
                IsUserChanged = true;
                ButtonA.Enabled = true;
            });
        }
    }
}
