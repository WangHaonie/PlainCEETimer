using System;
using System.Drawing;
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

        private bool CanRemoveChars;
        private readonly bool EnabledExpandable;
        private AppForm ParentForm;
        private PlainButton ButtonExpand;
        private event EventHandler UpdateContentRequested;

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
                this.AddControls(b => [ButtonExpand = b.Button("..", 20, 20, ButtonDetails_Click).With(x =>
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            CanRemoveChars = e.Modifiers == Keys.Control && e.KeyCode == Keys.V;
            e.SuppressKeyPress = e.KeyCode is Keys.Enter or Keys.Space;
            base.OnKeyDown(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (CanRemoveChars)
            {
                Text = Text.RemoveIllegalChars();
                CanRemoveChars = false;
            }

            base.OnTextChanged(e);
        }

        private void ButtonDetails_Click(object sender, EventArgs e)
        {
            if (EnabledExpandable)
            {
                ExpandableTextBox Dialog = new(GetShowBounds())
                {
                    Content = Text
                };

                Dialog.DialogResultAcquired += (_, dr) =>
                {
                    if (dr == DialogResult.Yes)
                    {
                        Text = Dialog.Content;
                    }
                };

                Dialog.ExtraKeyDownHandler += OnExpandableKeyDown;
                ParentForm.BindOverlayWindow(Dialog, () => GetShowBounds().Location);
                Dialog.Show(this);
            }
        }

        private Rectangle GetShowBounds()
        {
            var p = Parent.PointToScreen(Location);
            return new(p.X - 4, p.Y - 4, Width, 0);
        }
    }
}
