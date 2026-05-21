using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainTextBox : TextBox, IThemeAware
{
    private sealed class TextBoxFlyout(PlainTextBox parent) : AppForm
    {
        public string Content
        {
            get => m_Text;
            set => ContentBox.Text = value;
        }

        protected override AppWindowStyle Params => AppWindowStyle.RoundCornerSmall | AppWindowStyle.OnEscClosing | AppWindowStyle.ModelessDialog;

        private PlainTextBox ContentBox;
        private PlainButton ButtonClose;
        private PlainButton ButtonApply;
        private PlainLabel LabelCounter;
        private int TextLength;
        private string m_Text;
        private bool IsDark;
        private ThemeHelper themeHelper;

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
                ButtonClose = b.Button("×", 18, 20, (_, _) => Close()),
                ButtonApply = b.Button("√", 18, 20, ButtonApply_Click),
                LabelCounter = b.Label("0/0")
            ]);

            themeHelper = new(this);
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

        protected override void UpdateTheme(bool useDark, bool init)
        {
            IsDark = useDark;
            base.UpdateTheme(useDark, init);

            if (!init)
            {
                UpdateCounterColor();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ButtonApply_Click(null, null);
            }

            base.OnKeyDown(e);
        }

        protected override void Dispose(bool disposing)
        {
            themeHelper.Destroy();
            base.Dispose(disposing);
        }

        protected override void OnClosed()
        {
            parent.OnExpandableVisibleChanged(false);
            parent.Focus();
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            DialogEndResult = true;
            Close();
        }

        private void ContentBox_TextChanged(object sender, EventArgs e)
        {
            m_Text = ContentBox.Text;
            TextLength = ContentBox.Text.Clean().Length;
            LabelCounter.Text = TextLength + "/" + ConfigValidator.MaxCustomTextLength;
            UpdateCounterColor();
        }

        private void UpdateCounterColor()
        {
            LabelCounter.ForeColor = !ConfigValidator.IsInvalidCustomLength(TextLength)
                ? (IsDark ? Colors.DarkForeText : Color.Black)
                : Color.Red;
        }
    }

    public new string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;

            if (expandable && Child != null && !Child.IsDisposed)
            {
                Child.Content = value;
            }
        }
    }

    public event EventHandler<bool> ExpandableVisibleChanged;

    private bool initCall = true;
    private AppForm ParentForm;
    private PlainButton ButtonExpand;
    private TextBoxFlyout Child;
    private ThemeHelper themeHelper;
    private readonly bool expandable;
    private readonly Debouncer<EventArgs> debouncer;

    public PlainTextBox(bool isExpandable)
    {
        MaxLength = ConfigValidator.MaxCustomTextLength;

        if (expandable = isExpandable)
        {
            this.AddControls(b =>
            [
                ButtonExpand = b.Button("..", 18, 20, (_, _) =>
                {
                    Child = new(this);

                    Child.DialogEnd += (_, e) =>
                    {
                        if (e.Result == true)
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
                })
            ]);
        }

        debouncer = new(base.OnTextChanged);
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
        base.OnHandleCreated(e);

        if (expandable)
        {
            UpdateExpandButtonMargin();
        }

        themeHelper ??= new(this);
        ParentForm = this.FindParentForm();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateExpandButtonMargin();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        e.SuppressKeyPress = e.KeyCode is Keys.Enter or Keys.Space;
        base.OnKeyDown(e);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        if (initCall)
        {
            base.OnTextChanged(e);
            initCall = false;
            return;
        }

        debouncer.Debounce(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM.PASTE)
        {
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText().Clean();
                Input(text.Length + Text.Length, text);
            }

            return;
        }

        base.WndProc(ref m);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        debouncer.Destroy();
        base.Dispose(disposing);
    }

    private void OnParentLocationChanged(object sender, EventArgs e)
    {
        Child.Location = this.LocationToScreen(-4, -4);
    }

    private void UpdateExpandButtonMargin()
    {
        if (expandable)
        {
            /*

            TextBox 留白参考：

            c# - TextBox String/Text's Padding For Custom Control - Stack Overflow
            https://stackoverflow.com/a/38450341

            */

            Win32UI.SendMessage(Handle, NativeConstants.EM_SETMARGINS, NativeConstants.EC_RIGHTMARGIN, int.MakeLong(0, ButtonExpand.Width));
        }
    }

    private void OnExpandableVisibleChanged(bool visible)
    {
        ExpandableVisibleChanged?.Invoke(this, visible);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ForeColor = useDark ? Colors.DarkForeText : SystemColors.WindowText;
        BackColor = useDark ? Colors.DarkBackText : SystemColors.Window;
        ThemeManager.EnableDarkModeForControl(this, useDark ? SystemStyle.CfdDark : SystemStyle.Cfd, true);
    }
}
