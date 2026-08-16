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
    private sealed class TextBoxFlyout(PlainTextBox parent) : PlainFlyout
    {
        public string Content
        {
            get => ContentBox.Text;
            set => ContentBox.Text = value;
        }

        protected override AppWindowStyle Params
            => AppWindowStyle.RoundCornerSmall | AppWindowStyle.OnEscClosing | AppWindowStyle.ModelessDialog
                | AppWindowStyle.HideBeforeClose;

        protected override Point Offset => new(-4, -4);

        protected override Control OwnerControl => parent;

        private PlainTextBox ContentBox;
        private PlainButton ButtonClose;
        private PlainButton ButtonApply;
        private PlainTextCounter LabelCounter;

        public void Input(string text)
        {
            ContentBox.Input(ContentBox.Text.Clean().Length + text.Length, text);
        }

        protected override void OnInitializing()
        {
            base.OnInitializing();

            this.AddControls(b =>
            [
                ContentBox = b.TextArea(0, 0, null),
                ButtonClose = b.Button("×", 18, 20, (_, _) => Close()),
                ButtonApply = b.Button("√", 18, 20, ButtonApply_Click),
                LabelCounter = b.Counter(ContentBox, ConfigValidator.IsValidCustomLength)
            ]);
        }

        protected override void RunLayout(bool init, bool isHighDpi)
        {
            ContentBox.SetBounds(0, 0, ScaleToDpi(UnscaleToDpi(parent.Width, parent.DeviceDpi)), ScaleToDpi(110), BoundsSpecified.Size);
            ArrangeFirstControl(ContentBox, 4, 4);
            ArrangeCommonButtonsR(ButtonApply, ButtonClose, ContentBox, 0, 3);
            ArrangeControlYL(LabelCounter, ContentBox);
            CenterControlY(LabelCounter, ButtonApply);
            InitWindowSize(ButtonClose, 3, 3);

            if (init)
            {
                ContentBox.Text = parent.Text;
                ContentBox.Pin(AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);
                ButtonClose.Pin(AnchorStyles.Right | AnchorStyles.Bottom);
                ButtonApply.Pin(AnchorStyles.Right | AnchorStyles.Bottom);
                LabelCounter.Pin(AnchorStyles.Left | AnchorStyles.Bottom);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            parent.OnFlyoutVisibleChanged(e);
            base.OnVisibleChanged(e);
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
            parent.Focus();
            base.OnClosed();
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            DialogEndResult = true;
            Close();
        }
    }

    public new string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;

            if (hasFlyout && flyout?.IsDisposed == false)
            {
                flyout.Content = value;
            }
        }
    }

    public bool FlyoutVisible => flyout?.Visible ?? false;

    public PreferredColors PreferredColors { get; set; }

    public event EventHandler FlyoutVisibleChanged;

    private PlainButton ButtonExpand;
    private TextBoxFlyout flyout;
    private ThemeHelper themeHelper;
    private readonly bool hasFlyout;
    private readonly Debouncer debouncer;
    private readonly ActionInvoker<EventArgs> OnTextChangedInvoker;

    public PlainTextBox(bool hasFlyout = false)
    {
        MaxLength = ConfigValidator.MaxCustomTextLength;

        if (this.hasFlyout = hasFlyout)
        {
            this.AddControls(b =>
            [
                ButtonExpand = b.Button("..", 18, 20, (_, _) =>
                {
                    flyout = new(this);

                    flyout.WhenEnd(e =>
                    {
                        if (e.Result == true)
                        {
                            Text = flyout.Content;
                        }
                    });

                    flyout.Show();
                }).With(x =>
                {
                    x.Cursor = Cursors.Arrow;
                    x.Dock = DockStyle.Right;
                })
            ]);
        }

        debouncer = new(new ControlDebounceHelper(this));
        OnTextChangedInvoker = new(base.OnTextChanged);
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
        flyout?.Input(text);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (hasFlyout)
        {
            UpdateExpandButtonMargin();
        }

        themeHelper ??= new(this);
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
        debouncer.Debounce(OnTextChangedInvoker.WithArgs(e));
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WinUser.WM_PASTE)
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

    private void UpdateExpandButtonMargin()
    {
        if (hasFlyout)
        {
            /*

            TextBox 留白参考：

            c# - TextBox String/Text's Padding For Custom Control - Stack Overflow
            https://stackoverflow.com/a/38450341

            */

            Win32UI.SendMessage(Handle, WinUser.EM_SETMARGINS, WinUser.EC_RIGHTMARGIN,
                nint.MakeLong(0, ButtonExpand.Width));
        }
    }

    private void OnFlyoutVisibleChanged(EventArgs e)
    {
        FlyoutVisibleChanged?.Invoke(this, e);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        var pcs = PreferredColors;

        if (pcs != null)
        {
            var cp = useDark ? pcs.Dark : pcs.Light;
            ForeColor = cp.Fore;
            BackColor = cp.Back;
        }
        else
        {
            ForeColor = useDark ? Colors.DarkForeText : SystemColors.WindowText;
            BackColor = useDark ? Colors.DarkBackText : SystemColors.Window;
        }

        ThemeManager.EnableDarkModeForControl(this, useDark ? SystemStyle.CfdDark : SystemStyle.Cfd, true);
    }
}
