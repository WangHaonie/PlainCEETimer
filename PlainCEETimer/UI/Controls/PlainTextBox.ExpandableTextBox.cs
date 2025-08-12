using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls
{
    partial class PlainTextBox
    {
        private sealed class ExpandableTextBox(PlainTextBox parent) : AppForm
        {
            public string Content => ContentBox.Text;

            protected override AppFormParam Params => AppFormParam.RoundCorner | AppFormParam.OnEscClosing;

            public event EventHandler<DialogResult> DialogResultAcquired;

            private PlainTextBox ContentBox;
            private PlainButton ButtonClose;
            private PlainButton ButtonApply;
            private PlainLabel LabelCounter;
            private int TextLength;
            private static readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

            public void Show(PlainTextBox owner)
            {
                ContentBox.Tag = owner.Tag;

                owner.UpdateContentRequested += (_, _) =>
                {
                    if (!IsDisposed)
                    {
                        ContentBox.Text = owner.Text;
                    }
                };

                base.Show(owner);
            }

            protected override void OnInitializing()
            {
                base.OnInitializing();
                AutoSize = true;
                Location = parent.LocationToScreen(-4, -4);

                this.AddControls(b =>
                [
                    ContentBox = b.TextArea(default, 100, ContentBox_TextChanged),
                    ButtonClose = b.Button("×", 20, 20, (_, _) => CloseDialog()),
                    ButtonApply = b.Button("√", 20, 20, ButtonApply_Click),
                    LabelCounter = b.Label("0/0")
                ]);
            }

            protected override void StartLayout(bool isHighDpi)
            {
                ContentBox.Width = parent.Width;
                ContentBox.Text = parent.Text;
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

                parent.OnExpandableKeyDown?.Invoke(ContentBox, e, TextLength);
                base.OnKeyDown(e);
            }

            private void ButtonApply_Click(object sender, EventArgs e)
            {
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
    }
}
