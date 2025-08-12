using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls
{
    public sealed partial class PlainTextBox : TextBox
    {
        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                UpdateContentRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public Action<PlainTextBox, KeyEventArgs, int> OnExpandableKeyDown { get; set; }

        private readonly bool EnabledExpandable;
        private AppForm ParentForm;
        private PlainButton ButtonExpand;
        private event EventHandler UpdateContentRequested;

        private const int WM_PASTE = 0x0302;
        private const int EM_SETMARGINS = 0x00D3;
        private const int EC_RIGHTMARGIN = 0x0002;

        public PlainTextBox(bool expandable)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = Colors.DarkForeText;
                BackColor = Colors.DarkBackText;
            }

            MaxLength = Validator.MaxCustomTextLength;

            if (EnabledExpandable = expandable)
            {
                this.AddControls(b => [ButtonExpand = b.Button("..", 20, 20, (_, _) =>
                {
                    ExpandableTextBox Dialog = new(this);

                    Dialog.DialogResultAcquired += (_, dr) =>
                    {
                        if (dr == DialogResult.Yes)
                        {
                            Text = Dialog.Content;
                        }
                    };

                    ParentForm.BindOverlayWindow(Dialog, () => this.LocationToScreen(-4, -4));
                    Dialog.Show(this);
                }).With(x =>
                {
                    x.Cursor = Cursors.Arrow;
                    x.Dock = DockStyle.Right;
                })]);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushControl(this, NativeStyle.CfdDark);
            }

            base.OnHandleCreated(e);

            if (EnabledExpandable)
            {
                /*
                
                TextBox 留白参考：

                c# - TextBox String/Text's Padding For Custom Control - Stack Overflow
                https://stackoverflow.com/a/38450341

                */

                Natives.SendMessage(Handle, EM_SETMARGINS, new(EC_RIGHTMARGIN), new(ButtonExpand.Width << 16));
            }

            OnTextChanged(EventArgs.Empty);
            ParentForm = (AppForm)FindForm();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE)
            {
                if (Clipboard.ContainsText())
                {
                    SelectedText = Clipboard.GetText().RemoveIllegalChars();
                }

                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.SuppressKeyPress = e.KeyCode is Keys.Enter or Keys.Space;
            base.OnKeyDown(e);
        }
    }
}
