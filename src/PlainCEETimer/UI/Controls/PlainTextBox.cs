using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainTextBox : TextBox
{
    private sealed class TextBoxFlyout(PlainTextBox parent) : AppForm
    {
        public string Content
        {
            get => m_Text;
            set => ContentBox.Text = value;
        }

        protected override AppFormParam Params => AppFormParam.RoundCornerSmall | AppFormParam.OnEscClosing | AppFormParam.ModelessDialog;

        private PlainTextBox ContentBox;
        private PlainButton ButtonClose;
        private PlainButton ButtonApply;
        private PlainLabel LabelCounter;
        private int TextLength;
        private string m_Text;
        private static readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

        public void Input(string text)
        {
            ContentBox?.Input(TextLength + text.Length, text);
        }

        protected override void OnInitializing()
        {
            base.OnInitializing();
            Location = parent.LocationToScreen(-4, -4);

            this.AddControls(b =>
            [
                ContentBox = b.TextArea(0, 0, ContentBox_TextChanged),
                ButtonClose = b.Button("×", 18, 20, (_, _) => EndModelessDialog(false)),
                ButtonApply = b.Button("√", 18, 20, ButtonApply_Click),
                LabelCounter = b.Label("0/0")
            ]);
        }

        protected override void RunLayout(bool isHighDpi)
        {
            ContentBox.Text = parent.Text;
            ContentBox.SetBounds(0, 0, parent.Width, ScaleToDpi(110), BoundsSpecified.Size);
            ArrangeFirstControl(ContentBox, 4, 4);
            ArrangeCommonButtonsR(ButtonApply, ButtonClose, ContentBox, 0, 3);
            ArrangeControlYL(LabelCounter, ContentBox);
            CenterControlY(LabelCounter, ButtonApply);
            parent.OnExpandableVisibleChanged(true);
            InitWindowSize(ButtonClose, 3, 3);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ButtonApply_Click(null, null);
            }

            base.OnKeyDown(e);
        }

        protected override void OnClosed()
        {
            parent.OnExpandableVisibleChanged(false);
            parent.Focus();
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            EndModelessDialog(true);
        }

        private void ContentBox_TextChanged(object sender, EventArgs e)
        {
            m_Text = ContentBox.Text;
            TextLength = ContentBox.Text.RemoveIllegalChars().Length;
            LabelCounter.Text = TextLength + "/" + ConfigValidator.MaxCustomTextLength;
            LabelCounter.ForeColor = !ConfigValidator.IsInvalidCustomLength(TextLength) ? (IsDark ? Colors.DarkForeText : Color.Black) : Color.Red;
        }
    }

    public new string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;

            if (Expandable && Child != null && !Child.IsDisposed)
            {
                Child.Content = value;
            }
        }
    }

    public event EventHandler<bool> ExpandableVisibleChanged;

    private AppForm ParentForm;
    private PlainButton ButtonExpand;
    private TextBoxFlyout Child;
    private readonly bool Expandable;

    public PlainTextBox(bool expandable)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ForeColor = Colors.DarkForeText;
            BackColor = Colors.DarkBackText;
        }

        MaxLength = ConfigValidator.MaxCustomTextLength;

        if (Expandable = expandable)
        {
            this.AddControls(b => [ButtonExpand = b.Button("..", 18, 20, (_, _) =>
            {
                Child = new(this);

                Child.DialogEnd += dr =>
                {
                    if (dr == DialogResult.OK)
                    {
                        Text = Child.Content;
                    }

                    ParentForm.LocationChanged -= OnParentLocationChanged;
                };

                ParentForm.LocationChanged += OnParentLocationChanged;
                Child.Show(ParentForm);
            }).With(x =>
            {
                x.Cursor = Cursors.Arrow;
                x.Dock = DockStyle.Right;
            })]);
        }
    }

    public void Input(int totalLength, string text)
    {
        if (totalLength <= MaxLength)
        {
            SelectedText = text;
        }
    }

    public void InputFlyout(string text)
    {
        Child?.Input(text);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        const int EM_SETMARGINS = 0x00D3;
        const int EC_RIGHTMARGIN = 0x0002;

        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkModeForControl(this, NativeStyle.CfdDark, true);
        }

        base.OnHandleCreated(e);

        if (Expandable)
        {
            /*

            TextBox 留白参考：

            c# - TextBox String/Text's Padding For Custom Control - Stack Overflow
            https://stackoverflow.com/a/38450341

            */

            Win32UI.SendMessage(Handle, EM_SETMARGINS, EC_RIGHTMARGIN, int.MakeLong(0, ButtonExpand.Width));
        }

        OnTextChanged(EventArgs.Empty);
        ParentForm = this.FindParentForm();
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_PASTE = 0x0302;

        if (m.Msg == WM_PASTE)
        {
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText().RemoveIllegalChars();
                Input(text.Length + Text.Length, text);
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

    private void OnParentLocationChanged(object sender, EventArgs e)
    {
        Child.Location = this.LocationToScreen(-4, -4);
    }

    private void OnExpandableVisibleChanged(bool visible)
    {
        ExpandableVisibleChanged?.Invoke(this, visible);
    }
}
