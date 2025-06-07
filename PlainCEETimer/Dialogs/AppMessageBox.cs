using System.Drawing;
using System.Media;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Dialogs
{
    public sealed class AppMessageBox(SystemSound Sound, MessageButtons Buttons, bool AutoClose) : AppDialog(AppFormParam.KeyPreview)
    {
        private DialogResult Result;
        private readonly bool AutoCloseRequired = AutoClose;
        private readonly MessageButtons ButtonsEx = Buttons;
        private readonly SystemSound DialogSound = Sound;

        public DialogResult ShowCore(AppForm OwnerForm, string Message, string Title, Bitmap AppMessageBoxIcon)
        {
            SuspendLayout();
            AutoScaleDimensions = new(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            Text = Title;

            this.AddControls(b =>
            [
                b.Modify(PanelMain, 0, 0, 161, 40, null, c =>
                {
                    c.AutoSize = true;
                    c.AutoSizeMode = AutoSizeMode.GrowOnly;

                    c.AddControls(b =>
                    [
                        b.New<PictureBox>(6, 3, 32, 32, null, c =>
                        {
                            c.BackgroundImageLayout = ImageLayout.Zoom;
                            c.Image = AppMessageBoxIcon;
                        }),

                        b.Label(41, 3, Message, c => SetLabelAutoWrap(c, (int)(GetCurrentScreenRect().Width * 0.75)))
                    ]);
                }),

                b.Modify(ButtonA, 8, 41, 75, 23, null),
                b.Modify(ButtonB, 89, 41, 75, 23, null)
            ]);

            ResumeLayout(true);

            AlignControlsR(ButtonA, ButtonB, PanelMain);

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
                3200.AsDelay(_ => Invoke(Close));
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
