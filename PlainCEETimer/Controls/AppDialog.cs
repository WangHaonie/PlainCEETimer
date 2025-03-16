using PlainCEETimer.Modules;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class AppDialog : AppForm
    {
        protected Panel PanelMain { get; private set; }
        protected Button ButtonB { get; private set; }
        protected Button ButtonA { get; private set; }

        private bool IsUserChanged;

        private AppDialog()
        {
            InitializeComponent();
        }

        protected AppDialog(AppDialogProp Prop) : this()
        {
            SetProperties(Prop);
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

        protected virtual void ButtonA_Click(object sender, EventArgs e)
        {
            IsUserChanged = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected virtual void ButtonB_Click(object sender, EventArgs e)
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
            WhenLoaded(() =>
            {
                if (!ButtonA.Enabled)
                {
                    IsUserChanged = true;
                    ButtonA.Enabled = true;
                }
            });
        }

        private void InitializeComponent()
        {
            PanelMain = new();
            ButtonA = new();
            ButtonB = new();
            ButtonA.Click += ButtonA_Click;
            ButtonB.Click += ButtonB_Click;
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
