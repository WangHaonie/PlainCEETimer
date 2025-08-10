using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls
{
    public sealed class PlainTextBox : TextBox
    {
        private sealed class ExpandableTextBoxDialog(Rectangle parentBounds) : AppForm(AppFormParam.None | AppFormParam.RoundCorner | AppFormParam.OnEscClosing)
        {
            public string Content { get; set; }

            public event Action<PlainTextBox, KeyEventArgs, int> ExtraKeyDownHandler;
            public event EventHandler<DialogResult> DialogResultAcquired;

            private PlainTextBox ContentBox;
            private PlainButton ButtonClose;
            private PlainButton ButtonApply;
            private PlainLabel LabelCounter;
            private int TextLength;
            private static readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

            public void Show(Control owner)
            {
                ContentBox.Tag = owner.Tag;
                base.Show(owner);
            }

            protected override void OnInitializing()
            {
                base.OnInitializing();
                AutoSize = true;
                Location = parentBounds.Location;

                this.AddControls(b =>
                [
                    ContentBox = b.TextBox(default, false, ContentBox_TextChanged).With(x =>
                    {
                        x.Multiline = true;
                        x.Height = 150;
                        x.ScrollBars = ScrollBars.Vertical;
                    }),

                    ButtonClose = b.Button("×", 20, 20, (_, _) => CloseDialog()),
                    ButtonApply = b.Button("√", 20, 20, ButtonApply_Click),
                    LabelCounter = b.Label("0/0")
                ]);
            }

            protected override void StartLayout(bool isHighDpi)
            {
                ContentBox.Width = parentBounds.Width;
                ContentBox.Text = Content;
                ArrangeFirstControl(ContentBox, 4, 4);
                ArrangeCommonButtonsR(ButtonApply, ButtonClose, ContentBox, 0, 3);
                ArrangeControlYL(LabelCounter, ContentBox);
                CenterControlY(LabelCounter, ButtonApply);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ButtonApply_Click(null, null);
                }

                ExtraKeyDownHandler?.Invoke(ContentBox, e, TextLength);
                base.OnKeyDown(e);
            }

            private void ButtonApply_Click(object sender, EventArgs e)
            {
                Content = ContentBox.Text;
                CloseDialog(DialogResult.Yes);
            }

            private void ContentBox_TextChanged(object sender, EventArgs e)
            {
                TextLength = ContentBox.Text.RemoveIllegalChars().Length;
                LabelCounter.Text = TextLength + "/" + Validator.MaxCustomTextLength;
                LabelCounter.ForeColor = !Validator.IsInvalidCustomLength(TextLength) ? (IsDark ? Colors.DarkForeText : Color.Black) : Color.Red;
            }

            private void CloseDialog(DialogResult result = DialogResult.None)
            {
                DialogResultAcquired?.Invoke(this, result);
                Close();
            }
        }

        private bool CanRemoveChars;
        private readonly bool EnabledExpandable;
        private AppForm ParentForm;
        private PlainButton ButtonDetails;

        public Action<PlainTextBox, KeyEventArgs, int> OnExpandableKeyDown { get; set; }

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
                this.AddControls(b => [ButtonDetails = b.Button("..", 20, 20, ButtonDetails_Click).With(x =>
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

                Natives.SendMessage(Handle, EM_SETMARGINS, new(EC_RIGHTMARGIN), new(ButtonDetails.Width << 16));
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
                ExpandableTextBoxDialog Dialog = new(GetShowBounds())
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
