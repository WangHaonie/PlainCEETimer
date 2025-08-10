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
        private sealed class RichTextDialog(Rectangle parentBounds) : AppForm(AppFormParam.None | AppFormParam.RoundCorner | AppFormParam.OnEscClosing)
        {
            public string Content { get; set; }

            private TextBox ContentBox;
            private PlainButton ButtonClose;
            private PlainButton ButtonApply;
            private PlainLabel LabelCounter;
            private DialogResult Result;
            private static readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

            public new DialogResult ShowDialog()
            {
                base.ShowDialog();
                return Result;
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

                    ButtonClose = b.Button("×", 20, 20, (_, _) => Close()),
                    ButtonApply = b.Button("√", 20, 20, ButtonApply_Click),
                    LabelCounter = b.Label("0/0")
                ]);
            }

            private void ButtonApply_Click(object sender, EventArgs e)
            {
                Result = DialogResult.Yes;
                Content = ContentBox.Text;
                Close();
            }

            private void ContentBox_TextChanged(object sender, EventArgs e)
            {
                int count = ContentBox.Text.RemoveIllegalChars().Length;
                LabelCounter.Text = count + "/" + Validator.MaxCustomTextLength;
                LabelCounter.ForeColor = !Validator.IsInvalidCustomLength(count) ? (IsDark ? Colors.DarkForeText : Color.Black) : Color.Red;
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

                base.OnKeyDown(e);
            }
        }

        private bool CanRemoveChars;
        private readonly bool RichTextMode;
        private PlainButton ButtonDetails;

        private const int EM_SETMARGINS = 0x00D3;
        private const int EC_RIGHTMARGIN = 0x0002;

        public PlainTextBox(bool richText)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = Colors.DarkForeText;
                BackColor = Colors.DarkBackText;
            }

            MaxLength = Validator.MaxCustomTextLength;

            if (RichTextMode = richText)
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

            if (RichTextMode)
            {
                /*
                
                TextBox 留白参考：

                c# - TextBox String/Text's Padding For Custom Control - Stack Overflow
                https://stackoverflow.com/a/38450341

                */

                Natives.SendMessage(Handle, EM_SETMARGINS, new(EC_RIGHTMARGIN), new(ButtonDetails.Width << 16));
            }

            OnTextChanged(EventArgs.Empty);
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
            if (RichTextMode)
            {
                var p = Parent.PointToScreen(Location);

                RichTextDialog Dialog = new(new(p.X - 4, p.Y - 4, Width, 0))
                {
                    Content = Text
                };

                if (Dialog.ShowDialog() == DialogResult.Yes)
                {
                    Text = Dialog.Content;
                }
            }
        }
    }
}
